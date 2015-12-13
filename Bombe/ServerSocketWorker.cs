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
    /// <summary>
    /// Receives multiple connections from clients and communicates
    /// with them.
    /// </summary>
    internal class ServerSocketWorker : SocketWorker
    {
        protected readonly int MAX_CONNECTIONS = 104;
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
                //Bridge.sendInfoMessageToForm(se.Message);
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
            int counter = 0;
            foreach (Socket sock in connectionsList.Keys)
            {
                if (isAlive(sock))
                {
                    sock.Close();
                    counter++;
                }
            }
            Bridge.sendInfoMessageToForm(String.Format("Closed {0} connections with {1} clients.\n", 
                    counter, clientsCounter));
            connectionsList.Clear();
            clientsCounter = 0;

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
                string str = receiveData(socket);
                string[] command = getCommand(str);
                switch (command[0])
                {
                    case "newconnection":
                        Client client = new Client(++clientsCounter, socket.RemoteEndPoint.ToString(), 
                                DateTime.Now, Int32.Parse(command[1]));
                        sendData(socket, "clientaccepted:" + clientsCounter);
                        Bridge.sendInfoMessageToForm(String.Format(
                                "Client {0} connected with {1} cores.\n", clientsCounter, 
                                command[1]));
                        Bridge.sendInfoMessageToForm(String.Format(
                                "Core 1 of client {0} connected.\n", clientsCounter));
                        connectionsList.Add(socket, client);
                        //Bridge.newClientConnected(socket);
                        break;
                    case "newcore":
                        //Bridge.newClientConnected(socket);
                        connectionsList.Values.ElementAt(connectionsList.Values.Count - 1).increaseConnectedCores();
                        Bridge.sendInfoMessageToForm(String.Format(
                                "Core {0} of client {1} connected.\n",
                                connectionsList.Values.ElementAt(connectionsList.Values.Count - 1).connectedCores,
                                command[1]));
                        connectionsList.Add(socket, connectionsList.Values.ElementAt(connectionsList.Values.Count - 1));
                        break;
                }
                sockListener.BeginAccept(new AsyncCallback(establishConnection), null);
                Bridge.newClientConnected(socket);
                //sendData(connectionsList.Keys.ElementAt(connectionsList.Count - 1), "Hello new client!");
                //receiveData(connectionsList.Keys.ElementAt(connectionsList.Count - 1));
            }
            catch (Exception e)
            {
                //if (sockListener.Connected == true)
                //    Bridge.sendInfoMessageToForm(e.Message);
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