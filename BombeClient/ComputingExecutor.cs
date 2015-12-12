using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ComputingHelpers;
using EnigmaCryptography;

namespace BombeClient
{
    internal class ComputingExecutor : ComputingSide
    {
        protected bool isConnected = false;
        protected Thread computingThread;
        protected string encryptedMessage;

        protected int rotorsCount;
        protected string[] rotorsLayout;
        protected char[] notchPositions;

        public ComputingExecutor()
            : base()
        {
            Bridge.setComputingSide(this);
            new ClientSocketWorker();
        }

        public void changeClientStatus()
        {
            if (isConnected)
            {
                Bridge.socketWorker.closeConnection();
                computingThread.Abort();
                isConnected = false;
            }
            else
            {
                if (!Bridge.socketWorker.establishConnection())
                {
                    return;
                }
                computingThread = new Thread(startComputing);
                isConnected = true;
                computingThread.IsBackground = true;
                computingThread.Start();
            }
        }

        protected void startComputing()
        {
            while (isConnected)
            {
                string[] parameters = getCommand(newCommand());
                switch (parameters[0])
                {
                    case "done":
                        return;
                    case "compute":
                        compute(parameters);
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

        protected void setEncryptedMessage(string message)
        {
            encryptedMessage = message;
        }

        protected string newCommand()
        {
            return Bridge.socketWorker.receiveData();
        }

        protected void changeConnectionStatus()
        {
            Bridge.window.Dispatcher.Invoke((Action)(() =>
            {
                changeClientStatus();
            }));
        }

        protected void compute(string[] parameters)
        {
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsCount, 4, 
                    getOffsets(rotorsCount, parameters), rotorsLayout, notchPositions);
            breaker.initialize();
            if (breaker.tryBreak(encryptedMessage))
            {
                Bridge.socketWorker.sendData("success:" + breaker.encrypt(encryptedMessage));
            }
            else
            {
                Bridge.socketWorker.sendData("fail");
            }
        }

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
