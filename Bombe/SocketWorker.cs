using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;
using Enigma;
using ComputingHelpers;

namespace Bombe
{
    class SocketWorker
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8888);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            requestCount = 0;
            byte[] bytesFrom;
            Byte[] sendBytes;

            while ((true))
            {
                try
                {
                    Console.ReadLine();
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    sendBytes = SocketHelper.getBytes(requestCount.ToString());
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();

                    bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    string s = SocketHelper.getString(bytesFrom);
                    s = s.Substring(0, s.IndexOf('\0'));
                    //dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> Data from client - " + s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }
    }
}