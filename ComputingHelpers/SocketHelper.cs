using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;

namespace ComputingHelpers
{
    public static class SocketHelper
    {
        public static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] getBytes(byte b)
        {
            byte[] bytes = new byte[1];
            bytes[0] = b;
            return bytes;
        }

        public static byte[] getBytes(int i)
        {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)i;
            return bytes;
        }

        public static string getString(byte[] bytes, int iRx)
        {
            char[] chars = new char[iRx / sizeof(char)];
            if (iRx % 2 == 1) iRx -= 1;
            System.Buffer.BlockCopy(bytes, 0, chars, 0, iRx);
            return new string(chars);
        }

        public static bool isAlive(Socket socket)
        {
            return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
        }

        public static string getLocalIP()
        {
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
            if (addr.Length > 1)
                return addr[1].ToString();
            else
                return addr[0].ToString();
        }
    }
}
