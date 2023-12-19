using nTact.DataComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DemoMode
{
    public class DemoMode : IUpdateableForm
    {
        private GalilWrapper2 MC = null;
        private FormMain _frmMain = null;

        public Boolean DemoModeActive { get; set; } = false;
        public Int16 DemoModeCMDId { get; set; } = -1; //  [ 0=XJog  |  1=ZJog  |  2=PumpJog  |  3=RotaryJog  |  4=TurnOnInput  |  5=TurnOffInput  |  6=TurnOnOutput  |  7=TurnOffOutput ]
        public Boolean CMDWaiting { get; set; } = false;
        public Boolean CMDRunning { get; set; } = false;
        public Boolean CMDCompleted { get; set; } = false;
        public Double JoggingPosIncrementAMT { get; set; } = 0;

        public Double XAxisTargetPos { get; set; } = 0.0;
        private double XPos = 0;
        public Double XAxisPos { get { return XPos; } set { XPos = value; } }

        public Double ZAxisTargetPos { get; set; } = 0.0;
        public Double ZAxisPos { get; set; } = 0.0;

        public Double PumpAxisTargetPos { get; set; } = 0.0;
        public Double PumpAxisPos { get; set; } = 0.0;

        public Int16 ValveTarget { get; set; } = -1;
        public Int16 ValvePos { get; set; } = -1; //  [ 0=OFF  |  1=DISPENSE  |  2=RECHARGE  |  3=VENT  |  4=UNKNOWN ]

        public DemoMode(GalilWrapper2 frmMain) 
        { 
            XAxisPos = 0;
        }

        public void SetMC(GalilWrapper2 mc)
        {
            MC = mc;
        }

        public void UpdateStatus()
        {
            if ((!CMDCompleted && !CMDWaiting && CMDRunning) || (!CMDRunning && CMDWaiting))
            {
                //if (CMDRunning && CMDWaiting && CMDCompleted) { CMDWaiting = false; } //Handle Case Where We Accidentally Skip Setting Completed Flag
                if (!CMDRunning && CMDWaiting) { CMDRunning = true; CMDWaiting = false; } //Set Flags

                //Run Command
                switch (DemoModeCMDId)
                {
                    case 0:
                        //X-Axis Jog
                        XAxisPos = XAxisPos != XAxisTargetPos ? XAxisPos + JoggingPosIncrementAMT : XAxisPos;
                        CMDCompleted = XAxisTargetPos <= XAxisPos;
                        if (CMDCompleted) XAxisPos = XAxisTargetPos;
                        break;

                    case 1:
                        //Z-Axis Jog


                        CMDCompleted = true;
                        break;

                    case 2:
                        //Pump Jog


                        CMDCompleted = true;
                        break;

                    case 3:
                        //Rotary-Valve Jog


                        CMDCompleted = true;
                        break;

                    case 4:
                        //Turn ON Input


                        CMDCompleted = true;
                        break;

                    case 5:
                        //Turn OFF Input


                        CMDCompleted = true;
                        break;

                    case 6:
                        //Turn ON Output


                        CMDCompleted = true;
                        break;

                    case 7:
                        //Turn OFF Output


                        CMDCompleted = true;
                        break;
                }

                if (CMDCompleted && !CMDWaiting && CMDRunning)
                {
                    //Handle Anything After Completion
                    CMDRunning = false;
                }
            }
        }

        public Boolean JogXAxis(double dist, double vel, int dir = 1)
        {
            DemoModeCMDId = 0; //X-Axis Jog Command
            XAxisTargetPos = XAxisPos + (dist * dir);
            JoggingPosIncrementAMT = (vel * .001) * dir;
            CMDWaiting = true;
            return dir == 1 || dir == -1;
        }
    }
}
