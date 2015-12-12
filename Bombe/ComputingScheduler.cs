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
        public readonly int ALPHABET_LENGTH = Settings.ALPHABET_LENGTH;
        public readonly int MAX_CLIENTS = 104;
        public readonly int STATUSES_ARRAYS = 4; // MAX_CLIENTS / ALPHABET_LENGTH;

        protected PartsHandler partsHandler;
        protected bool isServerRunning = false;
        protected byte solutionStatus = 0;
        protected byte solutionAttemptCounter = 0;

        protected byte rotorsAmount = 6;
        //protected string encryptedMessage = "VKRO HO HGH ITZEAA";
        protected string encryptedMessage;
        protected string stopWord;
        protected string[] rotorsLayout;
        protected char[] notchPositions;

        protected int arrayActive = 0;
        protected byte[][] statuses;
        protected byte[] checkingGroups;
        protected int lastChecked = 0;
        protected int index = 0;
        protected bool isDone;
        protected int partsDoneOnIteration = 0;

        public ComputingScheduler() : base()
        {
            Bridge.setComputingSide(this);
            //this.Bridge = (ServerWindowLogicBridge)base.Bridge;
            //this.Bridge.setComputingSide(base.Bridge.computingSide);
            
            new ServerSocketWorker();
        }

        public void changeServerStatus()
        {
            if (isServerRunning)
            {
                Bridge.socketWorker.closeAllConnections();
            }
            else
            {
                Bridge.socketWorker.waitForConnections();
            }
            isServerRunning = !isServerRunning;
        }

        public void startBreaking()
        {
            getEnigmaConfiguration();
            if (rotorsAmount < 5)
            {
                Thread thread = new Thread(startEasyBreaking);
                thread.IsBackground = true;
                thread.Start();
                return;
            }
            partsHandler = new PartsHandler(Bridge.window, ALPHABET_LENGTH / 2, rotorsAmount - 5,
                    ALPHABET_LENGTH, STATUSES_ARRAYS);
            statuses = new byte[STATUSES_ARRAYS][];
            for (int i = 0; i < STATUSES_ARRAYS; i++)
            {
                statuses[i] = new byte[ALPHABET_LENGTH];
                Array.Clear(statuses[i], 0, statuses[i].Length);
            }
            arrayActive = 0;
            isDone = false;
            lastChecked = 0;
            index = 0;
            partsDoneOnIteration = 0;
            checkingGroups = new byte[rotorsAmount - 5];
            partsHandler.setAll(statuses, checkingGroups, arrayActive);
            solutionAttemptCounter++;

            if (solutionStatus == 0)
            {
                foreach (Socket socket in Bridge.socketWorker.connectionsList.Keys)
                {
                    startNewSchedulingThread(socket);
                }
            }
            solutionStatus = 1;
        }

        public void getEnigmaConfiguration()
        {
            rotorsAmount = Byte.Parse(Bridge.window.rotorsamount.Text);
            rotorsLayout = Bridge.window.getRotorsLayout();
            notchPositions = Bridge.window.getNotchPositions();
            stopWord = Bridge.window.getStopWord();
            encryptedMessage = Bridge.window.getMessage();
        }

        protected void startEasyBreaking()
        {
            EnigmaBreaker breaker = new EnigmaBreaker(rotorsAmount, rotorsAmount,
                    new byte[rotorsAmount], rotorsLayout, notchPositions);
            breaker.initialize();
            if (breaker.tryBreak(encryptedMessage))
            {
                Bridge.sendInfoMessageToForm("Easy success");
            }
            else
            {
                Bridge.sendInfoMessageToForm("Easy fail");
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
            int solutionAttemptCounterLocal = solutionAttemptCounter;
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
                Bridge.socketWorker.sendData(socket, getComputeCommand(num));
                string result = Bridge.socketWorker.receiveData(socket);
                if (solutionAttemptCounterLocal != solutionAttemptCounter)
                {
                    solutionAttemptCounterLocal = solutionAttemptCounter;
                    continue;
                }
                string[] array = getCommand(result);
                switch (array[0])
                {
                    case "fail":
                        changePartStatuses(arrayUsed, num, 2);
                        continue;
                    case "success":
                        solutionStatus = 2;
                        Bridge.sendInfoMessageToForm("Message decrypted! Message: " + array[1]);
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
            //if (arrayActive != arrayUsed) return;
            partsHandler.set(index % ALPHABET_LENGTH, 
                    (index / ALPHABET_LENGTH) + arrayUsed - arrayActive, status);
            setPart(arrayUsed, index, status);
            if (status == 0 && index < lastChecked) lastChecked = index;
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
            Bridge.socketWorker.sendData(socket, str.ToString());
        }

        protected void sendMessage(Socket socket)
        {
            Bridge.socketWorker.sendData(socket, "setmessage:" + encryptedMessage + ':' +
                stopWord);
        }

        protected string getComputeCommand(int index)
        {
            StringBuilder str = new StringBuilder(64);
            str.Append("compute");
            int append = index / ALPHABET_LENGTH;
            str.Append(":" + (index % ALPHABET_LENGTH));
            for (int i = 0; i < checkingGroups.Length; i++)
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
                if (value > 1 && index < ALPHABET_LENGTH) increasePartsDoneOnIteration();
                if (partsDoneOnIteration == ALPHABET_LENGTH && !isDone)
                {
                    Array.Clear(statuses[arrayActive], 0, statuses[arrayActive].Length);
                    arrayActive = (arrayActive + 1) % STATUSES_ARRAYS;
                    incrementLastChecked(0);
                    partsHandler.setAll(statuses, checkingGroups, arrayActive);
                    lastChecked -= ALPHABET_LENGTH;
                    resetPartsDoneOnIteration();
                    return;
                }
                statuses[(arrayUsed + (index / ALPHABET_LENGTH)) % STATUSES_ARRAYS]
                        [index % ALPHABET_LENGTH] = (byte)value;
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
                for (int i = 0; i < ALPHABET_LENGTH; i++)
                {
                    if (statuses[arrayActive][i] != 0) partsDoneOnIteration++;
                }
            }
        }
    }
}
