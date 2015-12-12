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
    /// <summary>
    /// Splits the task into pieces and distributes it between all
    /// the connected clients.
    /// </summary>
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

        /// <summary>
        /// COnstructor.
        /// </summary>
        public ComputingScheduler() : base()
        {
            Bridge.setComputingSide(this);
            //this.Bridge = (ServerWindowLogicBridge)base.Bridge;
            //this.Bridge.setComputingSide(base.Bridge.computingSide);
            
            new ServerSocketWorker();
        }

        /// <summary>
        /// Chenge the server status, between receive connections/close all
        /// connections.
        /// </summary>
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

        /// <summary>
        /// Start breaking process of task.
        /// </summary>
        public void startBreaking()
        {
            // If task is too easy to distribute it.
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

            // Start new thread for each connected client.
            if (solutionStatus != 1)
            {
                foreach (Socket socket in Bridge.socketWorker.connectionsList.Keys)
                {
                    startNewSchedulingThread(socket);
                }
            }
            solutionStatus = 1;
        }

        /// <summary>
        /// Get enigma configuration from the MainWindow.
        /// </summary>
        public void getEnigmaConfiguration()
        {
            rotorsAmount = Byte.Parse(Bridge.window.rotorsamount.Text);
            rotorsLayout = Bridge.window.getRotorsLayout();
            notchPositions = Bridge.window.getNotchPositions();
            stopWord = Bridge.window.getStopWord();
            encryptedMessage = Bridge.window.getMessage();
        }

        /// <summary>
        /// Break task, if it too easy (rotorsAmount is less that 5).
        /// </summary>
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

        /// <summary>
        /// Handler to receive new connection, and add new client to the
        /// breaking process.
        /// </summary>
        /// <param name="socket">Socket, assigned to the new client.</param>
        public void newClient(Socket socket)
        {
            if (solutionStatus == 1)
            {
                startNewSchedulingThread(socket);
            }
        }

        /// <summary>
        /// Starts new thread to handle clients.
        /// </summary>
        /// <param name="socket">Socket, assigned to the client.</param>
        protected void startNewSchedulingThread(Socket socket)
        {
            Thread thread = new Thread(useSingleClient);
            thread.IsBackground = true;
            thread.Start(socket);
        }

        /// <summary>
        /// Handler, to handle a single client and to communicate with him.
        /// </summary>
        /// <param name="sock">Socket, assigned with the client.</param>
        protected void useSingleClient(Object sock)
        {
            int solutionAttemptCounterLocal = solutionAttemptCounter;
            Socket socket = (Socket)sock;
            sendInitialMessages(socket);
            while (!isDone)
            {
                int num, arrayUsed;
                // Get next unprocessed part.
                lock (this)
                {
                    num = getUncheckedPart();
                    arrayUsed = arrayActive;
                }
                // Send task and wait for result.
                Bridge.socketWorker.sendData(socket, getComputeCommand(num));
                string result = Bridge.socketWorker.receiveData(socket);
                if (solutionAttemptCounterLocal != solutionAttemptCounter)
                {
                    solutionAttemptCounterLocal = solutionAttemptCounter;
                    continue;
                }
                // Process the result.
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

        /// <summary>
        /// Change a part's of task status.
        /// </summary>
        /// <param name="arrayUsed">Array used with particular part.</param>
        /// <param name="index">Index of part.</param>
        /// <param name="status">Status to set.</param>
        protected void changePartStatuses(int arrayUsed, int index, int status)
        {
            partsHandler.set(index % ALPHABET_LENGTH, 
                    (index / ALPHABET_LENGTH) + arrayUsed - arrayActive, status);
            setPart(arrayUsed, index, status);
            if (status == 0 && index < lastChecked) lastChecked = index;
        }

        /// <summary>
        /// Send messages to client with the commin information, 
        /// about the task.
        /// </summary>
        /// <param name="socket">Socket, assigned with client.</param>
        protected void sendInitialMessages(Socket socket)
        {
            sendMessage(socket);
            sendLayout(socket);
        }

        /// <summary>
        /// Send the layout of Enigma machine to a client.
        /// </summary>
        /// <param name="socket">Socket, assigned with client.</param>
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

        /// <summary>
        /// Send the encrypted message and stop word to a client.
        /// </summary>
        /// <param name="socket">Socket, assigned with client.</param>
        protected void sendMessage(Socket socket)
        {
            Bridge.socketWorker.sendData(socket, "setmessage:" + encryptedMessage + ':' +
                stopWord);
        }

        /// <summary>
        /// Generates the string with next command to a client.
        /// </summary>
        /// <param name="index">Index of part to use.</param>
        /// <returns>String with command.</returns>
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

        /// <summary>
        /// Synchronized method by the ComputingScheduler object, to get next unchecked part.
        /// </summary>
        /// <returns>Index of chosen unchecked part.</returns>
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
                lastChecked = index + 1;
                return index;
            }
        }

        /// <summary>
        /// Check, if checkGroup entirely checked.
        /// </summary>
        /// <param name="array">CheckGroup array to check.</param>
        /// <returns>CHeck result.</returns>
        protected bool checkForOnes(byte[] array)
        {
            return array.All(b => b != 0);
        }

        /// <summary>
        /// Increment checking groups counters.
        /// </summary>
        /// <param name="position">Posigion to increment.</param>
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

        /// <summary>
        /// Synchronized method by the ComputingScheduler object, 
        /// to change a part's status.
        /// </summary>
        /// <param name="arrayUsed">Index of array, used as basis to part.</param>
        /// <param name="index">Index of part.</param>
        /// <param name="value">New status of part.</param>
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
            }
        }

        /// <summary>
        /// Increase counter of parts, done on current iteration.
        /// </summary>
        protected void increasePartsDoneOnIteration()
        {
            lock (this)
            {
                partsDoneOnIteration++;
            }
        }

        /// <summary>
        /// Reset counter of parts, done on current iteration.
        /// </summary>
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
