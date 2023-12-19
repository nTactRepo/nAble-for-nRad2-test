using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using ACTETHERLib;
using System.Diagnostics;
using nAble;

namespace nTact.PLC
{
    public class QCPU2
    {

        public TimeSpan Shortest = new TimeSpan(9999999);
        public TimeSpan Longest = new TimeSpan(0);

        private object thisLock = new object();
        private readonly LogEntry _log = null;

        private bool _bReaderConnected = false;
        private bool _bWriterConnected = false;
        //private bool _bConnecting = false;
        private Thread _threadPLCWriter = null;
        private Thread _threadPLCReader = null;

        private ManualResetEvent _eventWriteRequested = new ManualResetEvent(false);
        private ManualResetEvent _eventWriteFinished = new ManualResetEvent(false);

        private ManualResetEvent _eventWriterThreadStarted = new ManualResetEvent(false);
        private ManualResetEvent _eventStopWriterThread = new ManualResetEvent(false);
        private ManualResetEvent _eventReaderThreadStarted = new ManualResetEvent(false);
        private ManualResetEvent _eventStopReaderThread = new ManualResetEvent(false);

        private ActQNUDECPUTCP _plcReader = null;
        private ActQNUDECPUTCP _plcWriter = null;
		private int _nSetDeviceRC = -1;

        private class WriteItem
        {

			public int ItemType { get; private set; } = 0;
			/// <summary>
			/// WriteItem for WriteDevice() writes
			/// </summary>
			/// <param name="szDevice"></param>
			/// <param name="lData"></param>
            public WriteItem(string szDevice, int lData)
            {
                Device = szDevice;
                Data = lData;
            }

			/// WriteItem for WriteDeviceRandom() writes
			public WriteItem(string szDeviceList, int lBlockSize, int [] lplDataBlock)
			{
				Device = szDeviceList;
				DataBlockSize = lBlockSize;
				DataBlock = new int[DataBlockSize];
				lplDataBlock.CopyTo(DataBlock,0);
				ItemType = 1;
			}


			~WriteItem()
			{
				if (ItemType == 1 && DataBlock != null)
				{
					DataBlock = null;
				}
			}

			public string Device { get; set; }
			public int Data { get; set; }

			public int[] DataBlock { get; set; } = null;
			public int DataBlockSize { get; set; } = -1;
		}
        private ConcurrentQueue<WriteItem> qSimpleWrites = new ConcurrentQueue<WriteItem>();

        public int DReadSize { get; private set; } = 1000;
        public int WReadSize { get; private set; } = 1000;
        public int BitReadSize { get; private set; } = 125; // M 0-511
        public int IOBitReadSize { get; private set; } = 25; // X/Y 0x0-0x18E
		public string ActHostAddress { get; set; } = "";
		public int ActTimeOut { get; set; } = 10000;

		public int ReadInterval { get; set; } = 250;  //ms between reads

		public bool ReadRecipes { get; set; } = false;

		public bool ReadRobotPositions { get; set; } = false;

		public QCPU2(LogEntry log)
        {
            _log = log;
        }

        public QCPU2(LogEntry log, string strAddr) : this(log)
        {
            ActHostAddress = strAddr;
        }

        public QCPU2(LogEntry log, string strAddr, int nTimeOut) : this (log, strAddr)
        {
            ActTimeOut = nTimeOut;
        }

        ~QCPU2()
        {
            Close();
        }

        public bool IsOpen
        {
            get { return _bReaderConnected && _bWriterConnected; }
        }

        public int Open()
        {
            lock (thisLock)
            {
                int nRetVal = -1;

                if (!IsOpen)
                {
                    if (0==StartReaderThread())
                    {
                        if (0==StartWriterThread())
                        {
                            _log.log(LogType.TRACE, Category.INFO, string.Format("QCPU2 Reader and Writer Threads started and connected to {0}.", ActHostAddress));
                            nRetVal = 0;
                        }
                        else
                        {
                            StopWriterThread(true);
                            StopReaderThread(true);
                            _log.log(LogType.TRACE, Category.INFO, string.Format("ERROR: QCPU2 Writer thread could not be started; Attempted IP: {0}.", ActHostAddress));
                        }
                    }
                    else
                    {
                        StopReaderThread(true);
                        _log.log(LogType.TRACE, Category.INFO, string.Format("ERROR: QCPU2 Reader thread could not be started; Attempted IP: {0}.", ActHostAddress));
                    }
                }

                return nRetVal;
            }
        }

        public int Close()
        {
            lock (thisLock)
            {
                int nRetVal = -1;
                int nRC = 0;

                nRC = StopWriterThread();
                nRC += StopReaderThread();

                if (nRC != 0)
                {
                    nRC = StopWriterThread(true);
                    nRC += StopReaderThread(true);
                }

                return nRetVal;
            }
        }


        #region ReaderThread
        private int StartReaderThread()
        {
            int nRetVal = -1;

            if (_threadPLCReader == null)
            {
                _eventStopReaderThread.Reset();
                _threadPLCReader = new Thread(new ThreadStart(ReaderThread));
                _threadPLCReader.SetApartmentState(ApartmentState.STA);
                _threadPLCReader.Name = "QCPU2 PLC Reader";
                _threadPLCReader.Start();

                if (!_eventReaderThreadStarted.WaitOne(30000))
                {
                    _threadPLCReader.Abort();
                    _threadPLCReader = null;
                    _bReaderConnected = false;
                    _log.log(LogType.TRACE, Category.INFO, "ERROR Could not start QCPU2 Reader Thread");
                    throw (new Exception());
                }
                nRetVal = 0;
            }
            else
            {
                string sMsg = "QCPU2 Reader thread already existed and shouldn't have!";
                _log.log(LogType.TRACE, Category.INFO, sMsg);
                throw (new Exception(sMsg));
            }
            return nRetVal;
        }
        private int StopReaderThread(bool bForce = false)
        {
            int nRetVal = 0;

            if (_threadPLCReader != null)
            {
                _eventStopReaderThread.Set();

                if (!_threadPLCReader.Join(10000))
                {
                    string sMsg = "QCPU2 Reader thread did not respond to shutdown request! Aborting!!";
                    _log.log(LogType.TRACE, Category.INFO, sMsg);
                    _threadPLCReader.Abort();
                    nRetVal = 1;
                }
                else
                {
                    nRetVal = 0;
                }
                _threadPLCReader = null;
            }
            _bReaderConnected = false;
            return nRetVal;
        }
		private void ReaderThread()
        {
            bool bFirstRead = true;
            DateTime StartTime;
            DateTime EndTime;
            TimeSpan ElapsedTime;
			int nReadSize = 63, nVacReadSize = 31;

			int[] lDData = new int[DReadSize];
			int[] lDData2 = new int[nReadSize];
			int[] lDData3 = new int[4];
			int[] lDData4 = new int[nVacReadSize];
			int[] lWData = new int[WReadSize];
			int[] lTData = new int[BitReadSize];
            int[] lMData = new int[BitReadSize];
            int[] lBData = new int[BitReadSize];
            int[] lLData = new int[BitReadSize];
            int[] lXData = new int[IOBitReadSize];
            int[] lYData = new int[IOBitReadSize];
            int[] lAlarmData = new int[20];
            int nPos, nRC = 0;
            short i, j;
            int nMask;
            try
            {
                _plcReader = new ActQNUDECPUTCP();
                _plcReader.ActHostAddress = ActHostAddress;
                _plcReader.ActTimeOut = 10000;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "QCPU2::ReaderThread() Could not create new ActQNUDECPUTCP Object (Reader)- " + ex.Message,"ERROR");
            }

            try
            {
				_log.log(LogType.TRACE, Category.INFO, "QCPU2::ReaderThread() - Attempting Connection to CPU", "Info");
                nRC = _plcReader.Open();
                if (nRC == 0)
                {
                    _plcReader.GetCpuType(out string strCpuName, out int nCpuCode);
					_log.log(LogType.TRACE, Category.INFO, $"QCPU2::ReaderThread() - Connected to CPU {strCpuName}.  CPU Code:({nCpuCode})", "Info");
					_bReaderConnected = true;
                }
                else
                {
					_log.log(LogType.TRACE, Category.INFO, $"QCPU2::ReaderThread() - Connection Failed to CPU RC:{nRC}", "ERROR");
					_bReaderConnected = false;
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "QCPU2::ReaderThread() Error during connect - " + ex.Message);
            }

            //notify caller that we are started and status can be checked
            Debug.WriteLine("Notifying starter that PLC Reader thread started at: " + DateTime.Now.ToLongTimeString());
            _eventReaderThreadStarted.Set();

            // do the Read loop
            if (_bReaderConnected)
            {
                while (!_eventStopReaderThread.WaitOne(0))
                {
					try
					{
						if (bFirstRead)
						{
							bFirstRead = false;
							Debug.WriteLine("ReaderThread.. FIRST READ at : " + DateTime.Now.ToLongTimeString(), "DEBUG");
						}
						StartTime = System.DateTime.Now;

						// do any sends...
						// Read 'D' Data (words)
						nRC = _plcReader.ReadDeviceBlock("D0", DReadSize, out lDData[0]);
						if (nRC == 0)
						{
							lDData.CopyTo(GlobalData.Dword, 0);
						}

						// Read 'D' Data (words) for Recipe
						if (ReadRecipes)
						{
							nRC = _plcReader.ReadDeviceBlock("D2897", nReadSize, out lDData2[0]);
							if (nRC == 0)
							{
								lDData2.CopyTo(GlobalData.Dword, 2897);
							}

							nRC = _plcReader.ReadDeviceBlock("D3700", nVacReadSize, out lDData4[0]);
							if (nRC == 0)
							{
								lDData4.CopyTo(GlobalData.Dword, 3700);
							}
						}

						if (ReadRobotPositions)
						{
							nRC = _plcReader.ReadDeviceRandom("D6002\nD6003\nD6006\nD6007", 4, out lDData3[0]);
							if (nRC == 0)
							{
							    GlobalData.Dword[6002] = lDData3[0];
							    GlobalData.Dword[6003] = lDData3[1];
							    GlobalData.Dword[6006] = lDData3[2];
							    GlobalData.Dword[6007] = lDData3[3];
							}
						}

						// Read 'W' Data (words)
						nRC = _plcReader.ReadDeviceBlock("W0", WReadSize, out lWData[0]);
						if (nRC == 0)
						{
							lWData.CopyTo(GlobalData.Wword, 0);
						}

						// Read 'T' Data 
						nRC = _plcReader.ReadDeviceBlock("T0", BitReadSize, out lTData[0]);
						if (nRC == 0)
						{
							lTData.CopyTo(GlobalData.Tword, 0);
						}

						// Read 'M' Local Bits
						nRC += _plcReader.ReadDeviceBlock("M0", BitReadSize, out lMData[0]);
						// Read 'B' Local Bits
						nRC += _plcReader.ReadDeviceBlock("B0", BitReadSize, out lBData[0]);
						// Read 'L' Local Bits
						nRC += _plcReader.ReadDeviceBlock("L0", BitReadSize, out lLData[0]);
						// Read 'X' Inputs
						nRC += _plcReader.ReadDeviceBlock("X0", IOBitReadSize, out lXData[0]);
						// Read 'Y' Outputs
						nRC += _plcReader.ReadDeviceBlock("Y0", IOBitReadSize, out lYData[0]);
						// Now write them all to the GlobalData area
						if (nRC == 0)
						{
							nPos = 0;
							for (i = 0; i < BitReadSize; i++)
							{
								for (j = 0; j < 16; j++)
								{
									nMask = 0x1 << j;
									GlobalData.Mbit[nPos] = (lMData[i] & nMask) == nMask;
									GlobalData.Bbit[nPos] = (lBData[i] & nMask) == nMask;
									GlobalData.Lbit[nPos] = (lLData[i] & nMask) == nMask;
									if (i < IOBitReadSize)
									{
										GlobalData.Xbit[nPos] = (lXData[i] & nMask) == nMask;
										GlobalData.Ybit[nPos] = (lYData[i] & nMask) == nMask;
									}
									nPos++;
								}
							}
						}

						////////////////////////////////////////////////////////////////////////
						// Do some time record keeping

						EndTime = DateTime.Now;
						ElapsedTime = EndTime.Subtract(StartTime);
						GlobalData.PollingTime = ElapsedTime;
						if (ElapsedTime < Shortest)
						{
							Shortest = ElapsedTime;
							Debug.WriteLine("New Shortest read: " + Shortest.ToString());
						}

						if (ElapsedTime > Longest)
						{
							Longest = ElapsedTime;
							Debug.WriteLine("New Longest read: " + Longest.ToString());
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"QCPU2: Exception while reading from PLC - {ex.Message}", "ERROR");
					}
					// pause before next read
					Thread.Sleep(ReadInterval);
				}
			}

            _bReaderConnected = false;
            _plcReader.Close();
            _plcReader = null;
            _log.log(LogType.TRACE, Category.INFO, "QCPU2::ReaderThread() Thread has ended.");

        }
        #endregion ReaderThread

        #region WriterThread
        private int StartWriterThread()
        {
            int nRetVal = -1;
            if (_threadPLCWriter == null)
            {
                _bWriterConnected = false;

                // Start the new WriterThread
                _threadPLCWriter = new Thread(new ThreadStart(WriterThread));
                _threadPLCWriter.SetApartmentState(ApartmentState.STA);  // needed to make the Mitsubishi 
                _threadPLCWriter.Name = "QCPU2 PLC Writer";
                _threadPLCWriter.Start();

                if (!_eventWriterThreadStarted.WaitOne(15000))
                {
                    _threadPLCWriter.Abort();
                    _threadPLCWriter = null;
                    _bWriterConnected = false;
                    nRetVal = -2;
                    _log.log(LogType.TRACE, Category.INFO, "ERROR Could not start QCPU2 Writer Thread");
                }
                else
                {
                    if (_bWriterConnected)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "QCPU2 WriterThread Started successfully: ");
                        // Writer thread is now started and awaiting a command
                        nRetVal = 0;
                    }
                }
            }
            else
            {
                _bWriterConnected = false;
                string sMsg = "QCPU2 Writer thread already existed and shouldn't have!";
                _log.log(LogType.TRACE, Category.INFO, sMsg);
                throw (new Exception(sMsg));
            }
            return nRetVal;
        }
        private int StopWriterThread(bool bForce = false)
        {
            int nRetVal = 0;

            if (_threadPLCWriter != null)
            {
                _eventStopWriterThread.Set();

                if (!_threadPLCWriter.Join(2000))
                {
                    string sMsg = "QCPU2 Writer thread did not respond to shutdown request! Aborting!!";
                    _log.log(LogType.TRACE, Category.INFO, sMsg, "Warning");
                    _threadPLCWriter.Abort();
                    nRetVal = 1;
                }
                else
                {
                    nRetVal = 0;
                }
                _threadPLCWriter = null;
            }
            _bWriterConnected = false;
            return nRetVal;
        }
        public int SetDevice(string szDevice, int lData)
        {
            lock (thisLock)
            {
                int nRetVal = -1;
				if (_bWriterConnected)
				{
					qSimpleWrites.Enqueue(new WriteItem(szDevice, lData));
					_eventWriteRequested.Set();
					if (!_eventWriteFinished.WaitOne(5000))
						_eventWriteFinished.Reset();
					nRetVal = _nSetDeviceRC;
					Debug.WriteLine(string.Format("Write of '{0}' to {1} : RC = {2}", lData, szDevice, nRetVal), "Debug");
				}
                return nRetVal;
            }
        }

		public int WriteDeviceRandom(string szDeviceList, int lSize, int [] lplData)
		{
			int nRetVal = 0;
			qSimpleWrites.Enqueue(new WriteItem(szDeviceList, lSize, lplData));
			_eventWriteRequested.Set();
			if (!_eventWriteFinished.WaitOne(5000))
				_eventWriteFinished.Reset();
			nRetVal = _nSetDeviceRC;
			Debug.WriteLine(string.Format("Write of {0} items to DeviceList {1} : RC = {2}", lSize, szDeviceList, nRetVal),"Debug");
			return nRetVal;
		}


		private void WriterThread()
        {
            int nIdx = -1;
            bool bContinue = true;
            int nRC = -1;

            // figure out what what we want to wait on.
            WaitHandle[] waitHandles = new WaitHandle[] { _eventWriteRequested, _eventStopWriterThread };

            _plcWriter = new ActQNUDECPUTCP();
            try
            {
                _plcWriter = new ActQNUDECPUTCP();
                _plcWriter.ActHostAddress = ActHostAddress;
                _plcWriter.ActTimeOut = ActTimeOut;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Could not create new ActQNUDECPUTCP Object (Writer)- " + ex.Message, "Error");
            }

            try
            {
                if (_plcWriter != null)
                {
                    nRC = _plcWriter.Open();
                    if (nRC == 0)
                    {
                        string strCpuName;
                        int nCpuCode;
                        _plcWriter.GetCpuType(out strCpuName, out nCpuCode);
                        _bWriterConnected = true;
                        _log.log(LogType.TRACE, Category.INFO, string.Format("QCPU2::WriterThread - Connected to {0} {{1}} at address: {2}", strCpuName, nCpuCode, ActHostAddress));
                    }
                    else
                    {
                        _bWriterConnected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "QCPU2::WriterThread() Error during connect" + ex.Message, "Error");
            }

            //notify caller that we are started and status can be checked
            _eventWriterThreadStarted.Set();

            while (bContinue && _plcWriter != null)
            {
                nIdx = WaitHandle.WaitAny(waitHandles);

                switch (nIdx)
                {
                    case 0: // Write
                    {
                        WriteItem writeItem;
                        while (qSimpleWrites.TryDequeue(out writeItem))
                        {
							switch(writeItem.ItemType )
							{
								case 0:
								{
									_nSetDeviceRC = _plcWriter.SetDevice(writeItem.Device, writeItem.Data);
								}
								break;
								case 1:
								{
									_nSetDeviceRC = _plcWriter.WriteDeviceRandom(writeItem.Device,writeItem.DataBlockSize, ref writeItem.DataBlock[0]);
								}
								break;
							}
						}
                        _eventWriteRequested.Reset();
                        _eventWriteFinished.Set();
                    }
                    break;
                    case 1: // Close
                    {
                        bContinue = false;
                    }
                    break;
                }

            }

            _bWriterConnected = false;
            _plcWriter.Close();
            _plcWriter = null;
            _log.log(LogType.TRACE, Category.INFO, "QCPU2::WriterThread() Thread has ended.","Info");
        }
        #endregion WriterThread

    }
}
