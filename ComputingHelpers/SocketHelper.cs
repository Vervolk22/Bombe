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

        public static string getString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char) + 2];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static bool isAlive(Socket s)
        {
            if (s.Connected == false || (s.Poll(1000, SelectMode.SelectRead) && s.Available == 0))
                return false;
            else
                return true;
        }
    }
}
