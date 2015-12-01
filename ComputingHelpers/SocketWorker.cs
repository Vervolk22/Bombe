using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace ComputingHelpers
{
    public abstract class SocketWorker
    {
        protected Window window;
        protected byte[] buffer = new byte[256];
        protected System.Timers.Timer aliveTimer;

        protected abstract void sendMessageToForm(string s);
        protected abstract void checkAlive(object source, System.Timers.ElapsedEventArgs e);

        public SocketWorker(Window window)
        {
            this.window = window;
            aliveTimer = new System.Timers.Timer(1000);
            aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkAlive);
            aliveTimer.Enabled = true;
        }

        protected static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        protected static byte[] getBytes(byte b)
        {
            byte[] bytes = new byte[1];
            bytes[0] = b;
            return bytes;
        }

        protected static string getString(byte[] bytes, int iRx)
        {
            char[] chars = new char[iRx / sizeof(char)];
            if (iRx % 2 == 1) iRx -= 1;
            System.Buffer.BlockCopy(bytes, 0, chars, 0, iRx);
            return new string(chars);
        }

        protected static bool isAlive(Socket socket)
        {
            return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
        }

        public string getLocalIP()
        {
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
            if (addr.Length > 1)
                return addr[1].ToString();
            else
                return addr[0].ToString();
        }

        public void sendData(Socket socket, string s)
        {
            try
            {
                sendMessageToForm("Message sent: " + s + "\n");
                byte[] byData = getBytes(s);
                socket.Send(getBytes((byte)byData.Length));
                socket.Send(byData);
            }
            catch (Exception se)
            {
                sendMessageToForm(se.Message);
            }
        }

        public string receiveData(Socket socket)
        {
            try
            {
                sendMessageToForm("Message received:\n");
                int iRx = socket.Receive(buffer, 1, SocketFlags.None);
                iRx = socket.Receive(buffer, buffer[0], SocketFlags.None);
                string str = getString(buffer, iRx);
                sendMessageToForm("---" + str + '\n');
                return str;
            }
            catch (Exception se)
            {
                sendMessageToForm(se.Message);
                return null;
            }
        }
    }
}
