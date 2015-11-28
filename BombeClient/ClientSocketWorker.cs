using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using EnigmaCryptography;
using ComputingHelpers;

namespace BombeClient
{
    internal class ClientSocketWorker
    {
        static void Main(string[] args)
        {
            TcpClient clientSocket = new TcpClient();
            clientSocket.Connect("localhost", 8888);
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] inStream = new byte[10025];
            byte[] outStream;

            while (true)
            {
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string s = SocketHelper.getString(inStream);
                s = s.Substring(0, s.IndexOf('\0'));
                int t = 0;
                Int32.TryParse(s, out t);
                s = (t + 1).ToString();
                Console.WriteLine(s);
                outStream = SocketHelper.getBytes(s);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
            //byte[] outStream = Encoding.ASCII.GetBytes(Console.ReadLine());
            //serverStream.Write(outStream, 0, outStream.Length);
            //serverStream.Flush();

            Console.ReadLine();
        }
    }
}

