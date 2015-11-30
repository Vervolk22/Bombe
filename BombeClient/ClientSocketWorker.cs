using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;
using EnigmaCryptography;
using ComputingHelpers;

namespace BombeClient
{
    internal class ClientSocketWorker
    {
        private MainWindow window;
        private Socket socket;
        private System.Timers.Timer aliveTimer;

        public ClientSocketWorker(MainWindow window)
        {
            this.window = window;
            aliveTimer = new System.Timers.Timer(1000);
            aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkAlive);
            aliveTimer.Enabled = true;
        }

        public bool establishConnection()
        {
            try
            {
                //create a new client socket ...
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szIPSelected = window.iplabel.Text;
                String szPort = window.port.Text;
                int alPort = System.Convert.ToInt16(szPort, 10);

                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                //System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Loopback;
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                
                socket.Connect(remoteEndPoint);
                /*IAsyncResult result = socket.BeginConnect(remoteEndPoint, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (!success)
                {
                    // NOTE, MUST CLOSE THE SOCKET

                    socket.Close();
                    throw new ApplicationException("Failed to connect server.");
                }*/
                sendMessageToForm(String.Format("Connected to {0}.\n", remoteEndPoint));
                window.status.Content = "connected";
                window.status.Foreground = System.Windows.Media.Brushes.Green;
                window.cmdReceiveConnections.Content = "Disconnect";
                //String szData = "Hello There";
                //byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                //socket.Send(byData);
                //new Thread(() => sendData(socket, "Hello There")).Start();
                return true;
                //sendData(socket, "Hello There");
            }
            catch (Exception e)
            {
                sendMessageToForm(e.Message);
                return false;
            }
        }

        private void checkAlive(object source, System.Timers.ElapsedEventArgs e)
        {
            if (socket != null && !SocketHelper.isAlive(socket))
            {
                closeConnection();
            }
        }

        public void closeConnection()
        {
            try
            {
                //socket.Close();
                socket.Disconnect(false);
                sendMessageToForm("Connection closed.\n");
                socket = null;
                window.status.Content = "not connected";
                window.status.Foreground = System.Windows.Media.Brushes.Red;
                window.cmdReceiveConnections.Content = "Connect";
            }
            catch (Exception e)
            {
                sendMessageToForm(e.Message);
            }
        }

        public void waitingForData()
        {

        }

        private void sendMessageToForm(string s)
        {
            try
            {
                window.Dispatcher.Invoke((Action)(() =>
                {
                    window.mainlist.AppendText(s);
                    window.mainlist.Focus();
                    window.mainlist.CaretIndex = window.mainlist.Text.Length;
                    window.mainlist.ScrollToEnd();
                }));
            }
            catch (Exception e)
            {

            }
        }

        internal void sendData(string s)
        {
            try
            {
                sendMessageToForm("Message sent: " + s + "\n");
                byte[] byData = SocketHelper.getBytes(s);
                socket.Send(byData);
            }
            catch (SocketException se)
            {
                sendMessageToForm(se.Message);
            }
        }

        internal string receiveData()
        {
            try
            {
                sendMessageToForm("Message received:\n");
                byte[] buffer = new byte[1024];
                int iRx = socket.Receive(buffer);
                string str = SocketHelper.getString(buffer, iRx);
                sendMessageToForm("---" + str + '\n');
                return str;
            }
            catch (SocketException se)
            {
                sendMessageToForm(se.Message);
                return null;
            }
        }
    }
}

