using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CommonLibrary.nTactServer
{
    public static class IPServices
    {
        public static string BroadcastAddressRequestStr = "NTACT IP_RQST:";
        public static string EndMessageTag = "{END}";

        public static string GetLocalIP(string name = "WAN", AddressFamily family = AddressFamily.InterNetwork)
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up && item.Name == name)
                {
                    foreach (UnicastIPAddressInformation uni in item.GetIPProperties().UnicastAddresses)
                    {
                        if (uni.Address.AddressFamily == family)
                        {
                            return uni.Address.ToString();
                        }
                    }
                }
            }

            return "";
        }

        public static bool IsIpAddressValid(string ipAddress)
        {
            bool valid = false;

            var ipEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ipAddr in ipEntry.AddressList)
            {
                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ipAddr.ToString() == ipAddress)
                    {
                        valid = true;
                        break;
                    }
                }
            }

            return valid;
        }
    }
}
