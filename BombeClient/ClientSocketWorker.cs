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
    /// <summary>
    /// Connects to the server and communicates with it.
    /// </summary>
    internal class ClientSocketWorker : SocketWorker
    {
        protected new MainWindow window;
        protected Socket socket;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">Main WPF window to interact with.</param>
        public ClientSocketWorker(MainWindow window)
            : base(window)
        {
            this.window = window;
        }

        /// <summary>
        /// Makes connection with a server.
        /// </summary>
        /// <returns>Result, if connection was set.</returns>
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

        /// <summary>
        /// Check is connection with the server alive. Will be called by a timer.
        /// </summary>
        /// <param name="source">Calling source (timer).</param>
        /// <param name="e">ElapsedEventArgs of a timer.</param>
        protected override void checkAlive(object source, System.Timers.ElapsedEventArgs e)
        {
            if (socket != null && !isAlive(socket))
            {
                closeConnection();
            }
        }
        
        /// <summary>
        /// Closes connection with the server.
        /// </summary>
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

        /// <summary>
        /// Send message to main window, about changes at socket level.
        /// </summary>
        /// <param name="s">Message to send.</param>
        protected override void sendMessageToForm(string s)
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

        /// <summary>
        /// Send data to the server. Possible, becauser client can handle
        /// only one connection with a server.
        /// </summary>
        /// <param name="s">String to send.</param>
        public void sendData(string s)
        {
            sendData(socket, s);
        }

        /// <summary>
        /// Receive data from the server. Possible, becauser client can handle
        /// only one connection with a server.
        /// </summary>
        /// <returns>Received data.</returns>
        public string receiveData()
        {
            return receiveData(socket);
        }
    }
}

