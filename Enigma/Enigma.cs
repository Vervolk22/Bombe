using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma
{
    public class Enigma
    {
        Rotor r1, r2, r3, r4, r5, reflector;

        public Enigma(int rotorsCount, int rotorsLayout, int[] offsets)
        {
            r1 = new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
            r2 = new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
            r3 = new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
            r4 = new Rotor("NQDJXVLSPHUFACKOIYRWZMEBGT", 'Q');
            r5 = new Rotor("CKPESOHXVUMJRFYALGQBTIDZWN", 'Q');
            reflector = new Rotor("YRUHQSLDPXNGOKMIEBFZCWVJAT", '\0');

            //J,Z

            r1.SetNextRotor(r2);
            r2.SetNextRotor(r3);
            r3.SetNextRotor(reflector);
            r2.SetPreviousRotor(r1);
            r3.SetPreviousRotor(r2);
            reflector.SetPreviousRotor(r3);

        }
    }
}
