using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaCryptography
{
    /// <summary>
    /// Enigma cryptography machine class.
    /// </summary>
    public class Enigma
    {
        protected string[] ROTORS_LAYOUT = {
                                                   "BDFHJLCPRTXVZNYEIWGAKMUSQO",
                                                   "AJDKSIRUXBLHWTMCQGZNPYFVOE",
                                                   "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
                                                   "NQDJXVLSPHUFACKOIYRWZMEBGT",
                                                   "CKPESOHXVUMJRFYALGQBTIDZWN",
                                                   "PGUYIOTMBXKFAHVRLZDNSWECJQ",
                                                   "YRUHQSLDPXNGOKMIEBFZCWVJAT"
                                               };
        protected char[] NOTCH_POSITIONS = { 'V', 'E', 'Q', 'D', 'W', 'O' };

        protected Rotor[] rotors;
        protected Rotor reflector, first;

        /// <summary>
        /// Enigma constructor.
        /// </summary>
        /// <param name="rotorsCount">Amount of rotors in machine.</param>
        /// <param name="offsets">Initial offsets of rotors before first use.</param>
        public Enigma(int rotorsCount, byte[] offsets)
        {
            rotors = new Rotor[rotorsCount];
            createRotors(rotorsCount, offsets);
            bindRotors(rotorsCount);
            first = rotors[0];
        }

        /// <summary>
        /// Allows to change rotor's commutation and notch positions.
        /// </summary>
        /// <param name="newLayout">Array of strings, that represents each 
        /// rotor's new commutation.</param>
        /// <param name="newPositions">Array of chars, where notches are placed.</param>
        public void changeEnigmaStructure(string[] newLayout, char[] newPositions)
        {
            this.ROTORS_LAYOUT = newLayout;
            this.NOTCH_POSITIONS = newPositions;
        }

        /// <summary>
        /// Creates a specified amount of rotors with specified initial offsets.
        /// </summary>
        /// <param name="rotorsCount">Amount of rotors in machine.</param>
        /// <param name="offsets">Array with initial rotor's offsets.</param>
        protected void createRotors(int rotorsCount, byte[] offsets)
        {
            for (int i = 0; i < rotorsCount; i++)
            {
                rotors[i] = new Rotor(ROTORS_LAYOUT[i], NOTCH_POSITIONS[i]);
                rotors[i].setOffset(offsets[i]);
            }
            reflector = new Rotor(ROTORS_LAYOUT[ROTORS_LAYOUT.Length - 1], '\0');
        }

        /// <summary>
        /// Binds each rotor with the next and previous ones.
        /// </summary>
        /// <param name="rotorsCount">Amount of rotors in machine.</param>
        protected void bindRotors(int rotorsCount)
        {
            for (int i = 0; i < rotorsCount; i++)
            {
                if (i != rotorsCount - 1)
                    rotors[i].SetNextRotor(rotors[i + 1]);
                if (i != 0)
                    rotors[i].SetPreviousRotor(rotors[i - 1]);
            }
            rotors[rotorsCount - 1].SetNextRotor(reflector);
            reflector.SetPreviousRotor(rotors[rotorsCount - 1]);
        }

        /// <summary>
        /// Encrypts a string with current rotors structure and offsets.
        /// </summary>
        /// <param name="s">String to be encrypted.</param>
        /// <returns>Encrypted string.</returns>
        public string encrypt(string s)
        {
            string result = "";
            foreach (char ch in s)
            {
                result += ch == ' ' ? ch : encrypt(ch);
            }
            return result;
        }

        /// <summary>
        /// Encrypts a single char.
        /// </summary>
        /// <param name="ch">Char to be encrypted.</param>
        /// <returns>Encrypted char.</returns>
        protected char encrypt(char ch)
        {
            ch = Char.ToUpper(ch);
            first.Move();
            first.PutDataIn(ch);
            char outt = first.GetDataOut();
            return outt;
            //return first.GetDataOut();
        }

    }
}
