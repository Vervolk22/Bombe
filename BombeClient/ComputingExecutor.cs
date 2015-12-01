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
    class ComputingExecutor
    {
        private MainWindow window;
        private ClientSocketWorker worker;
        private bool isConnected = false;
        private Thread computingThread;
        private string encryptedMessage;

        public ComputingExecutor(MainWindow window)
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

        public string getLocalIP()
        {
            return worker.getLocalIP();
        }

        private void startComputing()
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

        private string newCommand()
        {
            return worker.receiveData();
        }

        private void changeConnectionStatus()
        {
            window.Dispatcher.Invoke((Action)(() =>
            {
                changeClientStatus();
            }));
        }

        private string[] getCommand(string s)
        {
            if (s == null)
            {
                return new string[] { null };
            }
            return s.Split(':');
        }

        private void compute(string[] parameters)
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

        private byte[] getOffsets(int rotorsCount, string[] parameters)
        {
            byte[] array = new byte[rotorsCount];
            array.Initialize();
            for (int i = rotorsCount - 1; i > (rotorsCount - 1 ) - (parameters.Length - 2); i--)
            {
                array[i] = byte.Parse(parameters[2 + (rotorsCount - 1) - i]);
            }
            return array;
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
    }
}
