using Cognex.InSight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nAble.DataComm
{
    public class clsCognex
    {
        #region Properties

        public CvsInSight LeftCamera = new CvsInSight();
        public CvsInSight RightCamera = new CvsInSight();
        private string LeftIPAddress = "";
        private string RightIPAddress = "";
        private string UserID = "";
        private string Password = "";
        private BackgroundWorker AcquireResultsWorker = new BackgroundWorker();
        private AlignmentResults alignmentResult;
        public AlignmentResults AlignmentResult { get { return alignmentResult; } }

        private readonly LogEntry _log = null;

        public delegate void ResultsAcquiredEvenHandler(object o, EventArgs e);
        public static event ResultsAcquiredEvenHandler ResultsAcquired;

        public bool waitingOnResults = false;
        public bool WaitingOnResults { get { return waitingOnResults; } }
        private bool acquiringResults = false;
        public bool AcquiringResults { get { return acquiringResults; } }

        private String JobFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Job Files");
        public List<String> JobFiles = new List<string>();
        public bool IsConnected { get { return LeftCamIsConnected && RightCamIsConnected; } }
        public bool LeftCamIsConnected
        {
            get { return LeftCamera.State != CvsInSightState.NotConnected; }
        }
        public bool RightCamIsConnected
        {
            get { return RightCamera.State != CvsInSightState.NotConnected; }
        }
        #endregion

        #region Constructor
        public clsCognex(LogEntry log, string leftIPAddress, string rightIPAddress, string userID, string password)
        {
            _log = log;
            LeftIPAddress = leftIPAddress;
            RightIPAddress = rightIPAddress;
            UserID = userID;
            Password = password;
            alignmentResult = new AlignmentResults();
            LeftCamera.ResultsChanged += LeftCamera_ResultsChanged;
            LeftCamera.ConnectCompleted += LeftCamera_ConnectCompleted;
            RightCamera.ResultsChanged += RightCamera_ResultsChanged;
            RightCamera.ConnectCompleted += RightCamera_ConnectCompleted;
            AcquireResultsWorker.DoWork += AcquireResultsWorker_DoWork;
            AcquireResultsWorker.RunWorkerCompleted += AcquireResultsWorker_RunWorkerCompleted;
            Connect();
        }

        private void RightCamera_ConnectCompleted(object sender, CvsConnectCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Cognex.Connect() Right Camera Connect Completed.", "INFO");
        }

        private void LeftCamera_ConnectCompleted(object sender, CvsConnectCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Cognex.Connect() Left Camera Connect Completed.", "INFO");
        }

        private void AcquireResultsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            acquiringResults = false;
            ResultsAcquired(this, EventArgs.Empty);
        }

        private void AcquireResultsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "Acquire Results Worker";
            Trace.Listeners["nAble"].WriteLine($"AcquireResultsWorker started on ThreadID: {Thread.CurrentThread.ManagedThreadId}");
            alignmentResult.LeftImage = null;
            alignmentResult.LeftImageAcquired = false;
            alignmentResult.RightImage = null;
            alignmentResult.RightImageAcquired = false;
            alignmentResult.IsValid = false;
            waitingOnResults = true;
            RightCamera.ManualAcquire();
            LeftCamera.ManualAcquire();
        }

        private void RightCamera_ResultsChanged(object sender, EventArgs e)
        {
            try
            {
                CvsResultSet resultSet = RightCamera.Results;
            }
            catch (Exception ex)
            {

            }
        }

        private void LeftCamera_ResultsChanged(object sender, EventArgs e)
        {
            try
            {
                CvsResultSet resultSet = LeftCamera.Results;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Public Functions

        public bool Connect()
        {
            try
            {
                LeftCamera.Connect(LeftIPAddress, UserID, Password, false, true);
                RightCamera.Connect(RightIPAddress, UserID, Password, false, true);
                return true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Cognex.Connect() Failed. Exception:{ex.Message}", "ERROR");
                return false;
            }
        }
        public bool Disconnect()
        {
            LeftCamera.Disconnect();
            RightCamera.Disconnect();

            return !LeftCamIsConnected && !RightCamIsConnected;
        }

        public bool AcquireResults()
        {
            if (IsConnected)
            {
                acquiringResults = true;
                AcquireResultsWorker.RunWorkerAsync();
                return true;
            }
            return false;
        }
        public List<String> GetJobFiles()
        {
            if (LeftCamera.State == CvsInSightState.Online)
            {
                JobFiles.Clear();
                string[] jobFiles = LeftCamera.File.GetFileList();
                foreach (String file in jobFiles)
                {
                    if (file.Contains(".job"))
                        JobFiles.Add(file);
                }
            }

            return JobFiles;
        }
        public void LoadJobFile(string localFilePath)
        {
            if (LeftCamIsConnected)
            {
                LeftCamera.File.LoadJobFile(localFilePath);
                RightCamera.File.LoadJobFile(localFilePath);
            }
        }
        #endregion
    }
    public class AlignmentResults
    {
        public double Theta = 0;
        public double YOffset = 0;
        public bool IsValid = false;
        public bool LeftImageAcquired = false;
        public Bitmap LeftImage = new Bitmap(372, 496);
        public bool RightImageAcquired = false;
        public Bitmap RightImage = new Bitmap(372, 396);

        public override string ToString()
        {
            if (IsValid)
            {
                return $"Results: Theta: {Theta:#0.000} degs, Y-Offset: {YOffset:#0.000} mm";
            }
            else
            {
                return $"Results: Not Valid";
            }
        }
    }
}
