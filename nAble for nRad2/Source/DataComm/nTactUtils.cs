using System;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace nTact.DataComm
{
	public static class nTactUtils
	{
		static public bool Ping(string IPAddr)
		{
			bool bRetVal = false;
			string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);

			Ping pingSender = new Ping();
			PingOptions options = new PingOptions(64, true);
			PingReply reply = pingSender.Send(IPAddr, 2000, buffer, options);

			if (reply.Status == IPStatus.Success)
			{
				bRetVal = true;
			}

			return bRetVal;
		}

	}
}
