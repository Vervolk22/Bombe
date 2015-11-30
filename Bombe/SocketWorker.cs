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
        private const int MAX_CONNECTIONS = 2;

        private MainWindow window;
        private Socket sockListener;
        public Dictionary<Socket, int> connectionsList = new Dictionary<Socket, int>();
        private System.Timers.Timer aliveTimer;

        public SocketWorker(MainWindow window)
        {
            this.window = window;
            aliveTimer = new System.Timers.Timer(1000);
            aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkAliveConnections);
            aliveTimer.Enabled = true;
        }

        public void waitForConnections()
        {
            try
            {
                //create the listening socket...
                sockListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szPort = window.port.Text;
                int alPort = System.Convert.ToInt16(szPort, 10);
                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, alPort);
                //bind to local IP Address...
                sockListener.Bind(ipLocal);
                //start listening...
                sockListener.Listen(MAX_CONNECTIONS);
                // create the call back for any client connections...
                sockListener.BeginAccept(new AsyncCallback(establishConnection), null);
                window.status.Content = "online";
                window.status.Foreground = System.Windows.Media.Brushes.Green;
                window.cmdReceiveConnections.Content = "Close all connections";
                //cmdListen.Enabled = false;

            }
            catch (SocketException se)
            {
                sendMessageToForm(se.Message);
            }
        }

        public void closeAllConnections()
        {
            sockListener.Close();
            foreach (Socket sock in connectionsList.Keys)
            {
                if (SocketHelper.isAlive(sock))
                    sock.Close();
            }
            sendMessageToForm(String.Format("Closed {0} connections.\n", connectionsList.Count));
            connectionsList.Clear();

            window.status.Content = "offline";
            window.status.Foreground = System.Windows.Media.Brushes.Red;
            window.cmdReceiveConnections.Content = "Start receive connections";
        }

        public void establishConnection(IAsyncResult asyn)
        {
            try
            {
                connectionsList.Add(sockListener.EndAccept(asyn), connectionsList.Count + 1);
                sockListener.BeginAccept(new AsyncCallback(establishConnection), null);
                sendMessageToForm(String.Format("Client {0} connected.\n", connectionsList.Count));
                //sendData(connectionsList.Keys.ElementAt(connectionsList.Count - 1), "Hello new client!");
                //receiveData(connectionsList.Keys.ElementAt(connectionsList.Count - 1));
            }
            catch (Exception e)
            {
                if (sockListener.Connected == true)
                    sendMessageToForm(e.Message);
            }
        }

        public void closeConnection(Socket socket)
        {
            connectionsList.Remove(socket);
            socket.Close();
        }

        private void checkAliveConnections(object source, System.Timers.ElapsedEventArgs e)
        {
            int i = 0;
            while (i < connectionsList.Count)
            {
                //sendMessageToForm(connectionsList.Keys.ElementAt(i).Connected.ToString() + '\n');
                if (!SocketHelper.isAlive(connectionsList.Keys.ElementAt(i)))
                {
                    sendMessageToForm(String.Format("Client {0} disconnected.\n", connectionsList.Values.ElementAt(i)));
                    closeConnection(connectionsList.Keys.ElementAt(i));
                    continue;
                }
                i++;
            }
        }

        public void waitingForData()
        {

        }

        private void sendMessageToForm(string s)
        {
            window.Dispatcher.Invoke((Action)(() =>
            {
                window.mainlist.AppendText(s);
                window.mainlist.Focus();
                window.mainlist.CaretIndex = window.mainlist.Text.Length;
                window.mainlist.ScrollToEnd();
            }));
        }

        internal void sendData(Socket socket, string s)
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

        internal string receiveData(Socket socket)
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