using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using EnigmaCryptography;
using ComputingHelpers;

namespace Bombe
{
    internal class SocketWorker
    {
        public void waitForConnections()
        {

        }

        public void establishConnection()
        {

        }

        public void closeConnection()
        {

        }

        public void waitingForData()
        {

        }

        public string getLocalIP()
        {
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
            if (addr.Length > 1)
                return addr[1].ToString();
            else
                return addr[0].ToString();
        }
    }
}