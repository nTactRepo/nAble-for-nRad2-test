using nAble.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm
{
    public class ModbusMotorManager
    {
        private ModbusManager ModBus = null;

        public ModbusMotor ValveA_Motor { get; set; } = null;
        public ModbusMotor ValveB_Motor { get; set; } = null;
        public MachineSettingsII MS { get; set; } = null;

        public Boolean ValveAConnected { get; set; } = false;
        public Boolean ValveAHomed { get; set; } = false;
        public Boolean ValveAMoving { get; set; } = false;
        public int ValveAPosition { get; set; } = -1;
        public int ValveAStatus { get; set; } = -1;
        public int ValveAErrorCode { get; set; } = -1;

        public Boolean ValveBConnected { get; set; } = false;
        public Boolean ValveBHomed { get; set; } = false;
        public Boolean ValveBMoving { get; set; } = false;
        public int ValveBPosition { get; set; } = -1;
        public int ValveBStatus { get; set; } = -1;
        public int ValveBErrorCode { get; set; } = -1;

        public ModbusMotorManager(LogEntry log, MachineSettingsII machineSettings)
        {
            MS = machineSettings;
            ModBus = new ModbusManager();

            //ValveA_Motor = new ModbusMotor(MS.ValveAMotorID, MS.ModbusMotorCOM, MS.ModbusMotorBaud);
            ValveAConnected = ValveA_Motor.Connect();

            if (MS.DualPumpInstalled)
            {
                //ValveB_Motor = new ModbusMotor(MS.ValveBMotorID, MS.ModbusMotorCOM, MS.ModbusMotorBaud);
                ValveBConnected = ValveB_Motor.Connect();
            }
        }
    }

    public class ModbusMotor
    {
        public string COM { get; set; } = "";
        public int ID { get; set; } = 10;
        public int BAUD { get; set; } = 9600;

        public ModbusMotor(int id, string com, int baud)
        {
            COM = com;
            ID = id;
            BAUD = baud;
        }

        public Boolean Connect()
        {
            bool bRetVal = false;

            return bRetVal;
        }
    }
}
