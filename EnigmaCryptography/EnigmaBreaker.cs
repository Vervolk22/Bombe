using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaCryptography
{
    public class EnigmaBreaker : Enigma
    {
        private readonly string STOP_WORD = "RATEUSTEN";
        int iterationsCount;

        public EnigmaBreaker(int rotorsCount, byte[] offsets) : base(rotorsCount, offsets)
        {
            iterationsCount = pow(26, rotorsCount - 1);
        }

        public bool tryBreak(string s)
        {
            string message = deleteSpaces(s);
            int i = 0, offset = 0;
            char ch;
            while (i < iterationsCount)
            {
                //first.printOffset();
                //Console.WriteLine();
                i++;
                ch = encrypt(message[offset]);
                if (ch != STOP_WORD[offset])
                {
                    if (offset != 0)
                    {
                        first.restoreOffset();
                        offset = 0;
                    }
                }
                else
                {
                    offset++;
                    if (offset == 1)
                    {
                        first.saveOffset();
                    }
                    else if (offset == STOP_WORD.Length)
                    {
                        //Console.WriteLine("SOLUTION FOUNDED");
                        first.restoreOffset();
                        first.MoveBack();
                        //Console.WriteLine(encrypt(s));
                        //Console.WriteLine("exit");
                        return true;
                    }
                }
                if (i % 100 == 0)
                {
                    //Console.WriteLine("A: " + rotors[0].GetOffset() + "B: " + rotors[1].GetOffset() +
                    //    "C: " + rotors[2].GetOffset() + "D: " + rotors[3].GetOffset() + "E: " + rotors[4].GetOffset() + 
                    //    "I: " + i);
                }
            }

            return false;
        }

        protected int pow(int x, int e) 
        {
            int result = x;
            for (int i = 0; i < e - 1; i++)
            {
                result *= x;
            }
            return result;
        }

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
