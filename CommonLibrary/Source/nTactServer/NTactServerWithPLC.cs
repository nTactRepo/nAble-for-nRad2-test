using CommonLibrary.nTactServer.Messages;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonLibrary.Source.nTactServer.Messages.Batch;

namespace CommonLibrary.nTactServer
{
    public class NTactServerWithPLC : NTactServer
    {
        #region Properties

        public List<ushort> InputRegisters { get; set; } = new List<ushort>(3);
        public List<bool> InputDiscretes { get; set; } = new List<bool>(40);
        public List<ushort> HoldingRegisters { get; set; } = new List<ushort>(3);
        public List<bool> CoilDiscretes { get; set; } = new List<bool>(22);

        public bool[] CoilOneShots = new bool[22];

        #endregion

        #region Functions

        public NTactServerWithPLC(string ipAddress, int port) : base(ipAddress, port)
        {
            for (int i = 0; i < InputDiscretes.Capacity; i++)
            {
                InputDiscretes.Add(false);
            }
            for (int i = 0; i < InputRegisters.Capacity; i++)
            {
                InputRegisters.Add(0);
            }
            for (int i = 0; i < CoilDiscretes.Capacity; i++)
            {
                CoilDiscretes.Add(false);
            }
            for (int i = 0; i < HoldingRegisters.Capacity; i++)
            {
                HoldingRegisters.Add(0);
            }
        }

        override protected bool HandleMessage(MessageBase message)
        {
            message.Succeeded = false;
            bool handled = false;

            if (message is ReadModbusMessage rmm)
            {
                handled = true;
                message.Succeeded = HandleReadModbusMessage(rmm);
            }
            else if (message is WriteModbusAllMessage wamm)
            {
                handled = true;
                message.Succeeded = HandleWriteAllModbusMessage(wamm);
            }

            if (!handled)
            {
                handled = base.HandleMessage(message);
            }

            return handled;
        }

        private bool HandleReadModbusMessage(ReadModbusMessage rmm)
        {
            bool handled = true;
            rmm.InputDiscretes = InputDiscretes.ToList();
            rmm.InputRegisters = InputRegisters.ToList();
            return handled;
        }

        private bool HandleWriteAllModbusMessage(WriteModbusAllMessage wamm)
        {
            bool handled = true;
            CoilDiscretes =  wamm.Coils.ToList();
            HoldingRegisters = wamm.HoldingRegisters.ToList();
            return handled;
        }

        #endregion
    }
}
