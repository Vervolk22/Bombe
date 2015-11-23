using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaCryptography
{
    public class Enigma
    {
        private readonly string[] ROTORS_LAYOUT = {
                                                   "BDFHJLCPRTXVZNYEIWGAKMUSQO",
                                                   "AJDKSIRUXBLHWTMCQGZNPYFVOE",
                                                   "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
                                                   "NQDJXVLSPHUFACKOIYRWZMEBGT",
                                                   "CKPESOHXVUMJRFYALGQBTIDZWN",
                                                   "YRUHQSLDPXNGOKMIEBFZCWVJAT"
                                               };
        private readonly char[] NOTCH_POSITIONS = { 'V', 'E', 'Q', 'D', 'W' };

        Rotor[] rotors;
        Rotor reflector, first;

        public Enigma(int rotorsCount, int[] offsets)
        {
            rotors = new Rotor[rotorsCount];
            createRotors(rotorsCount, offsets);
            bindRotors(rotorsCount);
            first = rotors[0];
        }

        protected void createRotors(int rotorsCount, int[] offsets)
        {
            for (int i = 0; i < rotorsCount; i++)
            {
                rotors[i] = new Rotor(ROTORS_LAYOUT[i].Substring(offsets[i], 26 - offsets[i]) +
                                      ROTORS_LAYOUT[i].Substring(0, offsets[i]), NOTCH_POSITIONS[i]);
            }
            reflector = new Rotor(ROTORS_LAYOUT[ROTORS_LAYOUT.Length - 1], '\0');
        }

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

        public string encrypt(string s)
        {
            string result = "";
            foreach (char ch in s)
            {
                result += encrypt(ch);
            }
            return result;
        }

        protected char encrypt(char ch)
        {
            ch = Char.ToUpper(ch);
            first.Move();
            first.PutDataIn(ch);
            return first.GetDataOut();
        }

    }
}
