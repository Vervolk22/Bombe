using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Net;
using System.Net.Sockets;
using ComputingHelpers;
using EnigmaCryptography;

namespace Bombe
{
    class ComputingScheduler
    {
        private MainWindow window;
        private SocketWorker worker;
        private bool isServerRunning = false;

        private bool isDone;
        private byte[] statuses;
        private int lastChecked;

        public ComputingScheduler(MainWindow window)
        {
            this.window = window;
            worker = new SocketWorker(window);
        }

        public void changeServerStatus()
        {
            if (isServerRunning)
            {
                worker.closeAllConnections();
            }
            else
            {
                worker.waitForConnections();
            }
            isServerRunning = !isServerRunning;
        }

        public void startBreaking()
        {
            statuses = new byte[26];
            statuses.Initialize();
            isDone = false;
            lastChecked = 0;

            foreach (Socket socket in worker.connectionsList.Keys)
            {
                new Thread(useSingleClient).Start(socket);
            }
        }

        private void useSingleClient(Object sock)
        {
            Socket socket = (Socket)sock;
            while (!isDone)
            {
                byte index = (byte)getUncheckedPart();
                //sendMessageToForm("Sended: " + index + '\n');
                worker.sendData(socket, getQueryString(index));
                string result = worker.receiveData(socket);
                //sendMessageToForm("Received: " + result);
                string[] array = getCommand(result);
                switch (array[0])
                {
                    case "fail":
                        statuses[index] = 2;
                        continue;
                    case "success":
                        sendMessageToForm("Message decrypted! Message: " + array[1]);
                        statuses[index] = 3;
                        isDone = true;
                        break;
                }
            }
        }

        private string getQueryString(byte index)
        {
            string s = "compute:5:" + index + ':';
            s += "VKRO HO HGH ITZEAA";
            return s;
        }

        private int getUncheckedPart()
        {
            lock(this) {
                int index = lastChecked;
                while (statuses[index] != 0 && index <= statuses.Length)
                    index++;
                if (index == statuses.Length)
                {
                    isDone = true;
                    return -1;
                }
                else
                {
                    statuses[index] = 1;
                    return index;
                }
            }
        }

        public string getLocalIP()
        {
            return SocketHelper.getLocalIP();
        }

        private string[] getCommand(string s)
        {
            return s.Split(':');
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
