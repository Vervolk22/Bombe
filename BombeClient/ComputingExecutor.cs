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
        protected new MainWindow window;
        protected new ClientSocketWorker worker;
        protected bool isConnected = false;
        protected Thread computingThread;
        protected string encryptedMessage;

        public ComputingExecutor(MainWindow window) : base(window)
        {
            this.window = window;
            worker = new ClientSocketWorker(window);
        }

        public void changeClientStatus()
        {
            if (isConnected)
            {
                worker.closeConnection();
                computingThread.Abort();
                isConnected = false;
            }
            else
            {
                if (!worker.establishConnection())
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
                    case "wait":
                        Thread.Sleep(10000);
                        continue;
                    default:
                        changeConnectionStatus();
                        return;
                }
            }
        }

        protected void setEncryptedMessage(string message)
        {
            encryptedMessage = message;
        }

        protected string newCommand()
        {
            return worker.receiveData();
        }

        protected void changeConnectionStatus()
        {
            window.Dispatcher.Invoke((Action)(() =>
            {
                changeClientStatus();
            }));
        }

        protected void compute(string[] parameters)
        {
            int rotorsCount = int.Parse(parameters[1]);
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsCount, rotorsCount - 1, 
                    getOffsets(rotorsCount, parameters));
            if (breaker.tryBreak(encryptedMessage))
            {
                worker.sendData("success:" + breaker.encrypt(encryptedMessage));
            }
            else
            {
                worker.sendData("fail");
            }
            //sendMessageToForm("Sended answer: " + parameters[2]);
        }

        protected byte[] getOffsets(int rotorsCount, string[] parameters)
        {
            byte[] array = new byte[rotorsCount];
            array.Initialize();
            for (int i = rotorsCount - 1; i > (rotorsCount - 1 ) - (parameters.Length - 2); i--)
            {
                array[i] = byte.Parse(parameters[2 + (rotorsCount - 1) - i]);
            }
            return array;
        }

        protected override void sendMessageToForm(string s)
        {
            window.Dispatcher.Invoke((Action)(() =>
            {
                window.mainlist.AppendText(s);
                window.mainlist.Focus();
                window.mainlist.CaretIndex = window.mainlist.Text.Length;
                window.mainlist.ScrollToEnd();
            }));
        }
    }
}
