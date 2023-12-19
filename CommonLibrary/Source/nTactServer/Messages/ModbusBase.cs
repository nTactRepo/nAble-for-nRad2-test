using CommonLibrary.nTactServer.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public abstract class ModbusBase : MessageBase
    {
        #region Constants

        protected const char DataSeparator = ' ';
        protected const char RegCoilSeparator = ';';

        #endregion

        #region Common Functions

        public string MakeModbusString(List<bool> bits, List<ushort> regs)
        {
            string reply = "";
            bool first = true;

            foreach (var coil in bits)
            {
                var coilStr = $"{(coil ? 1 : 0)}";
                reply += first ? coilStr : $"{DataSeparator}{coilStr}";
                first = false;
            }

            reply += $"{RegCoilSeparator}";
            first = true;

            foreach (var reg in regs)
            {
                reply += first ? $"{reg}" : $"{DataSeparator}{reg}";
                first = false;
            }

            return reply;
        }

        public void ParseModbusMessageString(string replyBody, List<bool> bits, List<ushort> regs)
        {
            bits.Clear();
            regs.Clear();

            var parts = replyBody.Split(new char[] { RegCoilSeparator }).ToList();

            if (parts.Count != 2)
            {
                Trace.Listeners["nTact"].WriteLine("Problem parsing reply modbus message");
                return;
            }

            // Coils
            var coils = parts[0].Split(new char[] { DataSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var coilStr in coils)
            {
                bool coil = coilStr == "1" ? true : false;
                bits.Add(coil);
            }

            // Registers
            var regPart = parts[1].Split(new char[] { DataSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var regStr in regPart)
            {
                if (ushort.TryParse(regStr, out ushort reg))
                {
                    regs.Add(reg);
                }
            }
        }

        #endregion
    }
}
