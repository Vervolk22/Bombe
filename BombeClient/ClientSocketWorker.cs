using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void establishConnection()
        {
            try
            {
                //create a new client socket ...
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szIPSelected = window.ip.Text;
                String szPort = window.port.Text;
                int alPort = System.Convert.ToInt16(szPort, 10);

                //System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Loopback;
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                socket.Connect(remoteEndPoint);
                sendMessageToForm(String.Format("Connected to {0}.\n", remoteEndPoint));
                String szData = "Hello There";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                socket.Send(byData);
            }
            catch (SocketException se)
            {
                sendMessageToForm(se.Message);
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
                socket.Close();
                //socket.Disconnect(false);
                sendMessageToForm("Connection closed.\n");
                socket = null;
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
            window.Dispatcher.Invoke((Action)(() =>
            {
                window.mainlist.AppendText(s);
            }));
        }
    }
}

