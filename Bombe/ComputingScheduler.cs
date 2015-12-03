﻿using System;
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
    internal class ComputingScheduler : ComputingSide
    {
        protected new MainWindow window;
        protected new ServerSocketWorker worker;
        protected PartsHandler partsHandler;
        protected bool isServerRunning = false;
        protected byte solutionStatus = 0;
        protected string encryptedMessage = "VKRO HO HGH ITZEAA";

        private bool isDone;
        private byte[] statuses;
        private int lastChecked;

        public ComputingScheduler(MainWindow window) : base(window)
        {
            this.window = window;
            worker = new ServerSocketWorker(window, this);
            partsHandler = new PartsHandler(window, 13);
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
            partsHandler.setAll(26);

            foreach (Socket socket in worker.connectionsList.Keys)
            {
                solutionStatus = 1;
                startNewSchedulingThread(socket);
                //new Thread(useSingleClient).Start(socket);
            }
        }

        public void newClient(Socket socket)
        {
            if (solutionStatus == 1)
            {
                startNewSchedulingThread(socket);
            }
        }

        protected void startNewSchedulingThread(Socket socket)
        {
            Thread thread = new Thread(useSingleClient);
            thread.IsBackground = true;
            thread.Start(socket);
        }

        protected void useSingleClient(Object sock)
        {
            Socket socket = (Socket)sock;
            sendInitialMessages(socket);
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
                        partsHandler.set(index, 2);
                        statuses[index] = 2;
                        continue;
                    case "success":
                        solutionStatus = 2;
                        sendMessageToForm("Message decrypted! Message: " + array[1]);
                        partsHandler.set(index, 3);
                        statuses[index] = 3;
                        isDone = true;
                        break;
                    default:
                        setPart(index, 0);
                        return;
                }
            }
        }

        protected void sendInitialMessages(Socket socket)
        {
            worker.sendData(socket, "setmessage:" + encryptedMessage);
        }

        protected string getQueryString(byte index)
        {
            string s = "compute:5:" + index;
            return s;
        }

        protected int getUncheckedPart()
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
                    partsHandler.set(index, 1);
                    statuses[index] = 1;
                    lastChecked = index + 1;
                    return index;
                }
            }
        }

        protected void setPart(int index, byte value)
        {
            lock (this)
            {
                statuses[index] = value;
                if (value == 0)
                {
                    lastChecked = index;
                }
            }
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
