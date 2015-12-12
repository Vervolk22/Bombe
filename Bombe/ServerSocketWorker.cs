﻿using System;
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
    /// <summary>
    /// Receives multiple connections from clients and communicates
    /// with them.
    /// </summary>
    internal class ServerSocketWorker : SocketWorker
    {
        protected const int MAX_CONNECTIONS = 2; //TODO implement real max connections

        protected Socket sockListener;
        protected int clientsCounter = 0;
        public Dictionary<Socket, Client> connectionsList = new Dictionary<Socket, Client>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">Main WPF window to interact wit.</param>
        /// <param name="scheduler">ComputingScheduler of the server.</param>
        public ServerSocketWorker() : base()
        {
            Bridge.setSocketWorker(this);
        }

        /// <summary>
        /// Starts the waiting of incoming clients connections.
        /// </summary>
        public void waitForConnections()
        {
            try
            {
                //create the listening socket...
                sockListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szPort = Bridge.getPortText();
                int alPort = System.Convert.ToInt16(szPort, 10);
                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, alPort);
                //bind to local IP Address...
                sockListener.Bind(ipLocal);
                //start listening...
                sockListener.Listen(MAX_CONNECTIONS);
                // create the call back for any client connections...
                sockListener.BeginAccept(new AsyncCallback(establishConnection), null);
                Bridge.changeConnectionStatus(true);
            }
            catch (SocketException se)
            {
                Bridge.sendInfoMessageToForm(se.Message);
            }
        }

        /// <summary>
        /// Send message to main window, about changes at socket level.
        /// </summary>
        /// <param name="s">Message to send.</param>
        public override void sendInfoMessageToForm(string s)
        {
            try
            {
                Bridge.window.Dispatcher.Invoke((Action)(() =>
                {
                    Bridge.window.mainlist.AppendText(s);
                    Bridge.window.mainlist.Focus();
                    Bridge.window.mainlist.CaretIndex = Bridge.window.mainlist.Text.Length;
                    Bridge.window.mainlist.ScrollToEnd();
                }));
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Closes all alive connections, and set making of new connections to off.
        /// </summary>
        public void closeAllConnections()
        {
            sockListener.Close();
            foreach (Socket sock in connectionsList.Keys)
            {
                if (isAlive(sock))
                    sock.Close();
            }
            Bridge.sendInfoMessageToForm(String.Format("Closed {0} connections.\n", connectionsList.Count));
            connectionsList.Clear();

            Bridge.changeConnectionStatus(false);
        }

        /// <summary>
        /// Makes connection with a new client.
        /// </summary>
        /// <param name="asyn">Asynchronous call result.</param>
        public void establishConnection(IAsyncResult asyn)
        {
            try
            {
                Socket socket = sockListener.EndAccept(asyn);
                Client client = new Client(++clientsCounter, socket.RemoteEndPoint.ToString(), DateTime.Now);
                connectionsList.Add(socket, client);
                sockListener.BeginAccept(new AsyncCallback(establishConnection), null);
                Bridge.sendInfoMessageToForm(String.Format("Client {0} connected.\n", connectionsList.Count));
                Bridge.newClientConnected(socket);
                //sendData(connectionsList.Keys.ElementAt(connectionsList.Count - 1), "Hello new client!");
                //receiveData(connectionsList.Keys.ElementAt(connectionsList.Count - 1));
            }
            catch (Exception e)
            {
                if (sockListener.Connected == true)
                    Bridge.sendInfoMessageToForm(e.Message);
            }
        }

        /// <summary>
        /// Closes connection with a client.
        /// </summary>
        /// <param name="socket">Socket of alive connection.</param>
        public void closeConnection(Socket socket)
        {
            connectionsList.Remove(socket);
            socket.Close();
        }

        /// <summary>
        /// Checks all connections with clients, are they alive.
        /// </summary>
        /// <param name="source">Calling source (timer).</param>
        /// <param name="e">ElapsedEventArgs of a timer.</param>
        protected override void checkAlive(object source, System.Timers.ElapsedEventArgs e)
        {
            int i = 0;
            while (i < connectionsList.Count)
            {
                //Bridge.sendInfoMessageToForm(connectionsList.Keys.ElementAt(i).Connected.ToString() + '\n');
                if (!isAlive(connectionsList.Keys.ElementAt(i)))
                {
                    Bridge.sendInfoMessageToForm(String.Format("Client {0} disconnected.\n", connectionsList.Values.ElementAt(i)));
                    closeConnection(connectionsList.Keys.ElementAt(i));
                    continue;
                }
                i++;
            }
        }
    }
}