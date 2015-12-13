using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ComputingHelpers;
using EnigmaCryptography;

namespace BombeClient
{
    /// <summary>
    /// Receives parts of all breaking schedule from the server 
    /// and tries to break message with received part.
    /// </summary>
    internal class ComputingExecutor : ComputingSide
    {
        protected object lockObj = new Object();
        protected ClientSocketWorker sockWorker = null;
        protected int clientIndex = -1;
        protected int workersNum = 0;
        protected bool isConnected = false;
        protected Thread[] computingThreads;
        protected ClientSocketWorker[] workers;
        protected string encryptedMessage;

        protected int rotorsCount;
        protected string[] rotorsLayout;
        protected char[] notchPositions;

        /// <summary>
        /// Counstructor.
        /// </summary>
        public ComputingExecutor()
            : base()
        {
            Bridge.setComputingSide(this);
            new ClientSocketWorker();
        }

        /// <summary>
        /// Changes the client status, connected/disconnected from the server.
        /// </summary>
        public void changeClientStatus()
        {
            if (isConnected)
            {
                //Bridge.socketWorker.closeConnection();
                foreach (Thread thread in computingThreads)
                {
                    thread.Abort();
                }
                foreach (ClientSocketWorker worker in workers)
                {
                    worker.closeConnection();
                }
                Bridge.sendInfoMessageToForm("Connection with server closed.\n");
                workers = null;
                computingThreads = null;
                isConnected = false;
            }
            else
            {
                int coresAmount = getProcessorCoresAmount();
                ClientSocketWorker.resetConnectionsCounter();
                ClientSocketWorker.setCoresAmount(coresAmount);
                ClientSocketWorker worker = new ClientSocketWorker();
                if (!worker.establishConnection())
                {
                    return;
                }
                sockWorker = worker;
                isConnected = true;
                worker.sendData(getNewConnectionMessage(coresAmount));
                string newConnMessage = worker.receiveData();
                string[] connMessage = getCommand(newConnMessage);
                clientIndex = Int32.Parse(connMessage[1]);
                computingThreads = new Thread[coresAmount];
                workers = new ClientSocketWorker[coresAmount];
                workers[0] = worker;
                workersNum = 0;
                for (int i = 0; i < coresAmount; i++)
                {
                    computingThreads[i] = new Thread(startComputing);
                    computingThreads[i].IsBackground = true;
                    computingThreads[i].Start();
                }

            }
        }

        protected string getNewConnectionMessage(int coresAmount)
        {
            StringBuilder str = new StringBuilder(64);
            str.Append("newconnection:" + coresAmount);
            return str.ToString();
        }

        protected int getProcessorCoresAmount()
        {
            int coresAmount = 0;
            Bridge.window.Dispatcher.Invoke((Action)(() =>
            {
                coresAmount = Int32.Parse(Bridge.window.coressamount.Text);
            }));
            return coresAmount;
        }

        public int findProcessorCoresAmount()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            return coreCount;
        }

        /// <summary>
        /// Start receiving messages from the server and process them.
        /// </summary>
        protected void startComputing()
        {
            ClientSocketWorker worker;
            lock (lockObj)
            {
                if (sockWorker != null)
                {
                    worker = sockWorker;
                    sockWorker = null;
                }
                else
                {
                    worker = new ClientSocketWorker();
                    worker.establishConnection();
                    worker.sendData("newcore:" + clientIndex.ToString());
                    workers[++workersNum] = worker;
                }
            }
            while (isConnected)
            {
                string[] parameters = getCommand(newCommand(worker));
                switch (parameters[0])
                {
                    case "done":
                        return;
                    case "compute":
                        compute(parameters, worker);
                        break;
                    case "setmessage":
                        setEncryptedMessage(parameters[1]);
                        break;
                    case "setlayout":
                        setLayout(parameters);
                        break;
                    case "wait":
                        Thread.Sleep(10000);
                        continue;
                    default:
                        changeConnectionStatus();
                        return;
                }
            }
        }

        /// <summary>
        /// Set layout of Enigma machine, that wi trying to break.
        /// </summary>
        /// <param name="parameters">Enigma settings.</param>
        protected void setLayout(string[] parameters)
        {
            int rotors = Int32.Parse(parameters[1]);
            string[] layout = new string[rotors + 1];
            char[] notch = new char[rotors];
            for (int i = 0; i < rotors; i++)
            {
                layout[i] = parameters[i + 2];
                notch[i] = parameters[i + rotors + 3][0];
            }
            layout[rotors] = parameters[rotors + 2];

            this.rotorsCount = rotors;
            this.rotorsLayout = layout;
            this.notchPositions = notch;
        }

        /// <summary>
        /// Set encrypted message, that we are trying to break.
        /// </summary>
        /// <param name="message">Encrypted message.</param>
        protected void setEncryptedMessage(string message)
        {
            encryptedMessage = message;
        }

        /// <summary>
        /// Start waiting for new command from the server.
        /// </summary>
        /// <returns>Received command.</returns>
        protected string newCommand(ClientSocketWorker worker)
        {
            return worker.receiveData();
        }

        /// <summary>
        /// Change client status to connected/disconnected.
        /// </summary>
        protected void changeConnectionStatus()
        {
            Bridge.window.Dispatcher.Invoke((Action)(() =>
            {
                changeClientStatus();
            }));
        }

        /// <summary>
        /// A single try to break message, accordingly to received command.
        /// </summary>
        /// <param name="parameters">Received command.</param>
        protected void compute(string[] parameters, ClientSocketWorker worker)
        {
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsCount, 4, 
                    getOffsets(rotorsCount, parameters), rotorsLayout, notchPositions);
            breaker.initialize();
            if (breaker.tryBreak(encryptedMessage))
            {
                worker.sendData("success:" + breaker.encrypt(encryptedMessage));
            }
            else
            {
                worker.sendData("fail");
            }
        }

        /// <summary>
        /// Get offsets of rotors from the received command.
        /// </summary>
        /// <param name="rotorsCount">Amount of rotors.</param>
        /// <param name="parameters">Received parameters from the server.</param>
        /// <returns>Offsets of all rotors.</returns>
        protected byte[] getOffsets(int rotorsCount, string[] parameters)
        {
            byte[] array = new byte[rotorsCount];
            array.Initialize();
            for (int i = rotorsCount - 1; i > (rotorsCount - 1 ) - (parameters.Length - 1); i--)
            {
                array[i] = byte.Parse(parameters[2 + (rotorsCount - 2) - i]);
            }
            return array;
        }
    }
}
