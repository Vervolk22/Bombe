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
            }
            else
            {
                if (!worker.establishConnection())
                    return;
                computingThread = new Thread(startComputing);
                computingThread.Start();
            }
            isConnected = !isConnected;
        }

        public string getLocalIP()
        {
            return SocketHelper.getLocalIP();
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
                        //sendMessageToForm("Received command: " + parameters[2] + '\n');
                        compute(parameters);
                        break;
                    case "wait":
                        Thread.Sleep(10000);
                        continue;
                }
            }
        }

        private string newCommand()
        {
            return worker.receiveData();
        }

        private string[] getCommand(string s)
        {
            return s.Split(':');
        }

        private void compute(string[] parameters)
        {
            int rotorsCount = int.Parse(parameters[1]);
            byte offset = byte.Parse(parameters[2]);
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsCount, getOffsets(rotorsCount, offset));
            if (breaker.tryBreak(parameters[3]))
            {
                worker.sendData("success:" + breaker.encrypt(parameters[3]));
            }
            else
            {
                worker.sendData("fail");
            }
            //sendMessageToForm("Sended answer: " + parameters[2]);
        }

        private byte[] getOffsets(int rotorsCount, byte offset)
        {
            byte[] array = new byte[rotorsCount];
            array.Initialize();
            array[rotorsCount - 1] = offset;
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
