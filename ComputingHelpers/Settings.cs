using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingHelpers
{
    public static class Settings
    {
        internal static string[] rotorsLayout = {
                                                   "BDFHJLCPRTXVZNYEIWGAKMUSQO",
                                                   "AJDKSIRUXBLHWTMCQGZNPYFVOE",
                                                   "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
                                                   "NQDJXVLSPHUFACKOIYRWZMEBGT",
                                                   "CKPESOHXVUMJRFYALGQBTIDZWN",
                                                   "PGUYIOTMBXKFAHVRLZDNSWECJQ",
                                                   "YRUHQSLDPXNGOKMIEBFZCWVJAT",
                                                   "TBKOZSREHXUCPNWFLGDVQIJAYM",
                                                   "DINWSZACHRMVKQETYBOFUJXPGL",
                                                   "RKSLQNWCGDTHYAOEUMFZBVPXIJ",
                                                   "PEQJMVFTYRHLDCSIWNAUZGOXBK"
                                               };
        internal static char[] notchPositions = { 'V', 'E', 'Q', 'D', 'W', 'O', 'A', 'U', 'L', 'N' };
        public static readonly int ALPHABET_LENGTH = 26;
    }
}
