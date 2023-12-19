using nAble.Data;

namespace nRadLite.DataComm.IR
{
    public static class IRTransmitterFactory
    {
        public static IIRTransmitter CreateIRTransmitter(MachineSettingsII ms, bool isDemo)
        {
            return isDemo ? (IIRTransmitter)new DemoIRTransmitter(ms) : new IRTransmitter(ms);
        }
    }
}
