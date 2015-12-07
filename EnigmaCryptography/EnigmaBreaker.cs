using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaCryptography
{
    /// <summary>
    /// Brute-forces message, encrypted by enigma machine, to find original message.
    /// Solution found, when message starts with predetermined stop_word.
    /// </summary>
    public class EnigmaBreaker : Enigma
    {
        protected string STOP_WORD = "RATEUSTEN";
        protected int iterationsCount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rotorsCount">Amount of rotors in Enigma machine.</param>
        /// <param name="rotorsToCompute">Amount of rotors to brute-force. All 
        /// possible offsets of them will be checked. Offsets of other rotors
        /// will be constant.</param>
        /// <param name="offsets">Initial offsets of all rotors.</param>
        public EnigmaBreaker(int rotorsCount, int rotorsToCompute, byte[] offsets) : base(rotorsCount, offsets)
        {
            iterationsCount = pow(26, rotorsToCompute);
        }

        public EnigmaBreaker(int rotorsCount, int rotorsToCompute, byte[] offsets,
                string[] rotorsLayout, char[] notchPositions)
                : base(rotorsCount, offsets)
        {
            iterationsCount = pow(26, rotorsToCompute);
            this.ROTORS_LAYOUT = rotorsLayout;
            this.NOTCH_POSITIONS = notchPositions;
        }

        /// <summary>
        /// Change stop_word of original message.
        /// </summary>
        /// <param name="newStopWord">New stop word of the message to find.</param>
        public void changeStopWord(string newStopWord)
        {
            this.STOP_WORD = newStopWord;
        }

        /// <summary>
        /// Starts the brute-force breaking with predetermined amount of 
        /// iterations.
        /// </summary>
        /// <param name="s">Encrypted message to break.</param>
        /// <returns>Has solution been found by predetermined amount of 
        /// iterations.</returns>
        public bool tryBreak(string s)
        {
            string message = deleteSpaces(s);
            int i = 0, offset = 0;
            char ch;
            // Start breaking.
            while (i < iterationsCount)
            {
                i++;
                // Encrypt the next char.
                ch = encrypt(message[offset]);
                // If char not correspond to stop_word
                if (ch != STOP_WORD[offset])
                {
                    // If there are previous chars, that correspond to stop word, 
                    // restore rotors and checking position offsets.
                    if (offset != 0)
                    {
                        first.restoreOffset();
                        offset = 0;
                    }
                }
                // Otherwise set new offsets, and move to the next char to 
                // check, if it corresponds to the stop_word.
                else
                {
                    offset++;
                    // If it is the first matched char, save offsets, to 
                    // have the possibility to return here later, if 
                    // next char checks fails.
                    if (offset == 1)
                    {
                        first.saveOffset();
                    }
                    // Otherwise, if it is the last stop_message char, and it
                    // matches - solution found.
                    else if (offset == STOP_WORD.Length)
                    {
                        first.restoreOffset();
                        first.MoveBack();
                        return true;
                    }
                }
            }
            // All predetermined amount of iterations checked - solution
            // wasn't found.
            return false;
        }

        /// <summary>
        /// Finds the iterations amount to check: (value)^power.
        /// </summary>
        /// <param name="value">Value to raise to the power. It's a number of
        /// Letters in checked alphabet, now it's 26.</param>
        /// <param name="power">Power of expression.</param>
        /// <returns>Resulted amount of iterations.</returns>
        protected int pow(int value, int power) 
        {
            int result = value;
            for (int i = 0; i < power - 1; i++)
            {
                result *= value;
            }
            return result;
        }

        /// <summary>
        /// Deletes all the spaces from encrypted message, because
        /// spaces doesn't encrypt anyhow.
        /// </summary>
        /// <param name="s">Encrypted message.</param>
        /// <returns>Message with deleted spaces.</returns>
        protected string deleteSpaces(string s)
        {
            string result = "";
            foreach (char ch in s)
            {
                if (ch != ' ') result += ch;
            }
            return result;
        }
    }
}
