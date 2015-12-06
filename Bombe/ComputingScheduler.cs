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
    internal class ComputingScheduler : ComputingSide
    {
        protected int ALPHABET_LENGTH = 26;
        protected int MAX_CLIENTS = 104;
        protected int STATUSES_ARRAYS = 4; // MAX_CLIENTS / ALPHABET_LENGTH;

        protected new MainWindow window;
        protected new ServerSocketWorker worker;
        protected PartsHandler partsHandler;
        protected bool isServerRunning = false;
        protected byte solutionStatus = 0;

        protected byte rotorsAmount = 6;
        //protected string encryptedMessage = "VKRO HO HGH ITZEAA";
        protected string encryptedMessage = "SZLD YQ WFF CFZNFC";
        protected string stopWord = "RATEUSTEN";
        protected string[] rotorsLayout = {
                                                   "BDFHJLCPRTXVZNYEIWGAKMUSQO",
                                                   "AJDKSIRUXBLHWTMCQGZNPYFVOE",
                                                   "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
                                                   "NQDJXVLSPHUFACKOIYRWZMEBGT",
                                                   "CKPESOHXVUMJRFYALGQBTIDZWN",
                                                   "PGUYIOTMBXKFAHVRLZDNSWECJQ",
                                                   "YRUHQSLDPXNGOKMIEBFZCWVJAT"
                                               };
        protected char[] notchPositions = { 'V', 'E', 'Q', 'D', 'W', 'O' };

        protected int arrayActive = 0;
        protected byte[][] statuses;
        protected byte[] checkingGroups;
        protected int lastChecked = 0;
        protected int index = 0;
        protected bool isDone;
        protected int partsDoneOnIteration = 0;

        public ComputingScheduler(MainWindow window) : base(window)
        {
            this.window = window;
            worker = new ServerSocketWorker(window, this);
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
            if (rotorsAmount < 5)
            {
                Thread thread = new Thread(startEasyBreaking);
                thread.IsBackground = true;
                thread.Start();
                return;
            }
            partsHandler = new PartsHandler(window, ALPHABET_LENGTH / 2, rotorsAmount - 4,
                    ALPHABET_LENGTH, STATUSES_ARRAYS);
            statuses = new byte[STATUSES_ARRAYS][];
            for (int i = 0; i < STATUSES_ARRAYS; i++)
            {
                statuses[i] = new byte[ALPHABET_LENGTH];
                Array.Clear(statuses[i], 0, statuses[i].Length);
            }
            isDone = false;
            lastChecked = 0;
            checkingGroups = new byte[rotorsAmount - 5];
            partsHandler.setAll(statuses, arrayActive);

            foreach (Socket socket in worker.connectionsList.Keys)
            {
                solutionStatus = 1;
                startNewSchedulingThread(socket);
            }
        }

        protected void startEasyBreaking()
        {
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsAmount, rotorsAmount,
                    new byte[rotorsAmount], rotorsLayout, notchPositions);
            breaker.initialize();
            if (breaker.tryBreak(encryptedMessage))
            {
                sendMessageToForm("Easy success");
            }
            else
            {
                sendMessageToForm("Easy fail");
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
                int num, arrayUsed;
                lock (this)
                {
                    num = getUncheckedPart();
                    arrayUsed = arrayActive;
                }
                worker.sendData(socket, getComputeCommand(num));
                string result = worker.receiveData(socket);
                string[] array = getCommand(result);
                switch (array[0])
                {
                    case "fail":
                        changePartStatuses(arrayUsed, num, 2);
                        continue;
                    case "success":
                        solutionStatus = 2;
                        sendMessageToForm("Message decrypted! Message: " + array[1]);
                        changePartStatuses(arrayUsed, num, 3);
                        isDone = true;
                        break;
                    default:
                        changePartStatuses(arrayUsed, num, 0);
                        return;
                }
            }
        }

        protected void changePartStatuses(int arrayUsed, int index, int status)
        {
            partsHandler.set(index % ALPHABET_LENGTH, 
                    (index / ALPHABET_LENGTH) + arrayUsed - arrayActive, status);
            setPart(arrayUsed, index, status);
        }

        protected void sendInitialMessages(Socket socket)
        {
            sendMessage(socket);
            sendLayout(socket);
        }

        protected void sendLayout(Socket socket)
        {
            StringBuilder str = new StringBuilder(512);
            str.Append("setlayout:" + rotorsAmount);
            for (int i = 0; i <= rotorsAmount; i++)
            {
                str.Append(':' + rotorsLayout[i]);
            }
            for (int i = 0; i < rotorsAmount; i++)
            {
                str.Append(":" + notchPositions[i]);
            }
            worker.sendData(socket, str.ToString());
        }

        protected void sendMessage(Socket socket)
        {
            worker.sendData(socket, "setmessage:" + encryptedMessage + ':' +
                stopWord);
        }

        protected string getComputeCommand(int index)
        {
            StringBuilder str = new StringBuilder(64);
            str.Append("compute");
            int append = index / ALPHABET_LENGTH;
            str.Append(":" + (index % ALPHABET_LENGTH));
            for (int i = checkingGroups.Length - 1; i >= 0; i--)
            {
                index = checkingGroups[i] + append;
                append = index / ALPHABET_LENGTH;
                str.Append(":" + (index % ALPHABET_LENGTH));
            }
            return str.ToString();
        }

        protected int getUncheckedPart()
        {
            lock (this)
            {
                index = lastChecked;
                while (statuses[(arrayActive + (index / ALPHABET_LENGTH)) % STATUSES_ARRAYS]
                        [index % ALPHABET_LENGTH] != 0)
                {
                    index++;
                }
                partsHandler.set(index % ALPHABET_LENGTH, index / ALPHABET_LENGTH, 1);
                setPart(arrayActive, index, 1);
                //statuses[(arrayActive + (index / ALPHABET_LENGTH)) % STATUSES_ARRAYS]
                //        [index % ALPHABET_LENGTH] = 1;
                lastChecked = index + 1;
                return index;
            }
        }

        protected bool checkForOnes(byte[] array)
        {
            return array.All(b => b != 0);
        }

        protected void incrementLastChecked(int position)
        {
            if (position >= checkingGroups.Length)
            {
                isDone = true;
                return;
            }

            checkingGroups[position]++;
            if (checkingGroups[position] == ALPHABET_LENGTH)
            {
                checkingGroups[position] = 0;
                incrementLastChecked(position + 1);
            }
        }

        protected void setPart(int arrayUsed, int index, int value)
        {
            lock (this)
            {
                if (value > 1) increasePartsDoneOnIteration();
                if (partsDoneOnIteration == 26)
                {
                    resetPartsDoneOnIteration();
                    Array.Clear(statuses[arrayActive], 0, statuses[arrayActive].Length);
                    arrayActive = (arrayActive + 1) % STATUSES_ARRAYS;
                    incrementLastChecked(0);
                    partsHandler.setAll(statuses, arrayActive);
                    lastChecked -= ALPHABET_LENGTH;
                }
                statuses[(arrayUsed + (index / ALPHABET_LENGTH)) % STATUSES_ARRAYS]
                        [index % ALPHABET_LENGTH] = 1;
                /*if (value == 0)
                {
                    lastChecked = index;
                }*/
            }
        }

        protected void increasePartsDoneOnIteration()
        {
            lock (this)
            {
                partsDoneOnIteration++;
            }
        }

        protected void resetPartsDoneOnIteration()
        {
            lock (this)
            {
                partsDoneOnIteration = 0;
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
