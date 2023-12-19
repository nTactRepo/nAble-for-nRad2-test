using nAble;
using nAble.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nTact.DataComm
{
    //public class KeyenceLaserComm
    //{
    //    #region Inner Class

    //    public class HeadData
    //    {
    //        public int HeadNumber;
    //        public double Value;

    //        public bool Go;
    //        public bool Hi;
    //        public bool Lo;
    //        public bool Alarm;
    //        public bool Invalid;
    //        public bool Waiting;

    //        public HeadData(LKIF2.LKIF_FLOATVALUE_OUT data)
    //        {
    //            HeadNumber = data.outNo;
    //            Value = data.value;

    //            Go = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_VALID;
    //            Hi = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_RANGEOVER_P;
    //            Lo = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_RANGEOVER_N;
    //            Alarm = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_ALARM;
    //            Invalid = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_INVALID;
    //            Waiting = data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_WAITING;
    //        }
    //    }

    //    #endregion

    //    #region Member Data

    //    private const int numHeads = 4;
    //    private const int numPrograms = 8;

    //    private string _ipAddress = "";
    //    private GalilWrapper2 _galil = null;
    //    private HeadData[] _headValues = new HeadData[numHeads];
    //    private readonly LogEntry _log = null;

    //    #endregion

    //    #region Properties

    //    public bool IsConnected { get; set; } = false;
    //    public bool Head1GO { get { return _headValues[0].Go; } set { _headValues[0].Go = value; } }
    //    public bool Head1HI { get { return _headValues[0].Hi; } set { _headValues[0].Hi = value; } }
    //    public bool Head1LO { get { return _headValues[0].Lo; } set { _headValues[0].Lo = value; } }
    //    public bool Head1Alarm { get { return _headValues[0].Alarm; } set { _headValues[0].Alarm = value; } }
    //    public bool Head1Invalid { get { return _headValues[0].Invalid; } set { _headValues[0].Invalid = value; } }
    //    public bool Head1Waiting { get { return _headValues[0].Waiting; } set { _headValues[0].Waiting = value; } }
    //    public bool Head1Value { get { return _headValues[0].Waiting; } set { _headValues[0].Waiting = value; } }
    //    public bool Head2GO { get { return _headValues[1].Go; } set { _headValues[1].Go = value; } }
    //    public bool Head2HI { get { return _headValues[1].Hi; } set { _headValues[1].Hi = value; } }
    //    public bool Head2LO { get { return _headValues[1].Lo; } set { _headValues[1].Lo = value; } }
    //    public bool Head2Alarm { get { return _headValues[1].Alarm; } set { _headValues[1].Alarm = value; } }
    //    public bool Head2Invalid { get { return _headValues[1].Invalid; } set { _headValues[1].Invalid = value; } }
    //    public bool Head2Waiting { get { return _headValues[1].Waiting; } set { _headValues[1].Waiting = value; } }
    //    public double Head2Value { get { return _headValues[1].Value; } set { _headValues[1].Value = value; } }

    //    public Boolean ReadingLasers { get; set; } = false;

    //    #endregion

    //    #region Functions

    //    public KeyenceLaserComm(LogEntry log, GalilWrapper2 galil)
    //    {
    //        _log = log;
    //        _galil = galil;
    //    }

    //    public bool Connect(string ipAddress)
    //    {
    //        _log.log(LogType.TRACE, Category.DEBUG, "Got here connect");
    //        try
    //        {
    //            LKIF2.LKIF_OPENPARAM_ETHERNET openParam = new LKIF2.LKIF_OPENPARAM_ETHERNET();
    //            openParam.IPAddress.S_addr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
    //            var result = LKIF2.LKIF2_OpenDeviceETHER(ref openParam);
    //            _log.log(LogType.TRACE, Category.DEBUG, "Got here CONNECTED!!!!");
    //            IsConnected = result == LKIF2.RC.RC_OK;
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.log(LogType.TRACE, Category.DEBUG, $"Got exception starting laser: {ex.Message}");
    //        }
    //        return IsConnected;
    //    }

    //    public bool Disconnect()
    //    {
    //        var result = LKIF2.LKIF2_CloseDevice();
    //        IsConnected = result == LKIF2.RC.RC_OK;

    //        if (result != LKIF2.RC.RC_OK)
    //        {

    //        }

    //        return IsConnected;
    //    }

    //    public bool ZeroAndReadLasers(int numLasers, ref double leftLaserVal, ref double rightLaserVal)
    //    {
    //        ResetLaserValues(numLasers);
    //        Thread.Sleep(500);
    //        leftLaserVal = ReadLasers(numLasers)[0];
    //        rightLaserVal = ReadLasers(numLasers)[0];
    //        _headValues[0].Value = ReadLasers(numLasers)[0];
    //        _headValues[1].Value = ReadLasers(numLasers)[1];
    //        if (leftLaserVal == null || leftLaserVal == 0.0)
    //            return false;
    //        if (rightLaserVal == null || rightLaserVal == 0.0)
    //            return false;
    //        return true;
    //    }

    //    public bool GetLaserValues(int numLasers, ref double leftLaserVal, ref double rightLaserVal)
    //    {
    //        leftLaserVal = ReadLasers(numLasers)[0];
    //        rightLaserVal = ReadLasers(numLasers)[0];
    //        _headValues[0].Value = ReadLasers(numLasers)[0];
    //        _headValues[1].Value = ReadLasers(numLasers)[1];
    //        if (leftLaserVal == null || leftLaserVal == 0.0)
    //            return false;
    //        if (rightLaserVal == null || rightLaserVal == 0.0)
    //            return false;
    //        return true;
    //    }

    //    private List<double> ReadLasers(int totalLaserCount)
    //    {
    //        List<double> lRetVals = new List<double>();
    //        for (int i = 0; i <= totalLaserCount; i++)
    //        {
    //            lRetVals.Add(((HeadData)ReadHeadStatus(i)).Value);
    //        }
    //        return lRetVals;
    //    }

    //    public HeadData ReadHeadStatus(int headNumber)
    //    {
    //        if (!IsConnected)
    //        {
    //            return null;
    //        }

    //        if (headNumber < 0 || headNumber >= numHeads)
    //        {
    //            _log.log(LogType.TRACE, Category.INFO, $"Tried to read the head status for an unknown head: {headNumber}");
    //        }

    //        LKIF2.LKIF_FLOATVALUE_OUT lkHeadData = new LKIF2.LKIF_FLOATVALUE_OUT();
    //        var result = LKIF2.LKIF2_GetCalcDataSingle(headNumber, ref lkHeadData);

    //        if (result == LKIF2.RC.RC_OK)
    //        {
    //            _headValues[headNumber] = new HeadData(lkHeadData);
    //        }
    //        else
    //        {
    //            _log.log(LogType.TRACE, Category.INFO, $"Tried to read data for head {headNumber}, and got error code: {result}");
    //        }

    //        return _headValues[headNumber];
    //    }

    //    public bool SetProgramNumber(int programNumber)
    //    {
    //        if (!IsConnected)
    //            return false;

    //        if (programNumber < 0 || programNumber >= numPrograms)
    //        {
    //            _log.log(LogType.TRACE, Category.INFO, $"Tried to set an invalid program number: {programNumber}.  Resetting to zero.");
    //            programNumber = 0;
    //        }

    //        var result = LKIF2.LKIF2_SetProgramNo(programNumber);

    //        if (result == LKIF2.RC.RC_OK)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            _log.log(LogType.TRACE, Category.INFO, $"Tried to set program number {programNumber}, and got error code: {result}");
    //        }

    //        return false;
    //    }

    //    public int GetProgramNumber()
    //    {
    //        int programNumber = -1;

    //        if (IsConnected)
    //        {
    //            var result = LKIF2.LKIF2_GetProgramNo(ref programNumber);

    //            if (result == LKIF2.RC.RC_OK)
    //            {
    //                //programNumber = (int)(programNumber ^ 0xffffff00);
    //                programNumber = programNumber & 0xff;
    //            }
    //            else
    //            {
    //                programNumber = -1;
    //                _log.log(LogType.TRACE, Category.INFO, $"Tried to read the current program number, and got error code: {result}");
    //            }
    //        }

    //        return programNumber;
    //    }

    //    public bool ResetLaserValues(int totalLaserCount)
    //    {
    //        LKIF2.RC result;
    //        LKIF2.RC result2;
    //        LKIF2.RC result3;
    //        LKIF2.RC result4;
    //        if (IsConnected)
    //        {
    //            switch (totalLaserCount)
    //            {
    //                case 1:
    //                    result = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01);
    //                    return !result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01));

    //                case 2:
    //                    result = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01);
    //                    if (result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01)))
    //                        return false;
    //                    result2 = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_02);
    //                    return !result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01));

    //                case 4:
    //                    result = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01);
    //                    if (result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01)))
    //                        return false;
    //                    result2 = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_02);
    //                    if (result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_02)))
    //                        return false;
    //                    result3 = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_03);
    //                    if (result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_03)))
    //                        return false;
    //                    result4 = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_04);
    //                    return !result4.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01));

    //                case 0://Default
    //                    result = LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01);
    //                    return !result.HasFlag(LKIF2.LKIF2_SetResetMulti(LKIF2.LKIF_OUTNO.LKIF_OUTNO_01));
    //            }
    //        }
    //        return false;
    //    }

    //    #endregion
    //}
}
