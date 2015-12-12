using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingHelpers
{
    /// <summary>
    /// Contains default Enigma settings.
    /// </summary>
    public static class Settings
    {
        public static readonly string[] rotorsLayout = {
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
                                                   "YRUHQSLDPXNGOKMIEBFZCWVJAT"
                                               };
        public static readonly char[] notchPositions = { 'V', 'E', 'Q', 'D', 'W', 'O', 'A', 'U', 'L', 'N' };
        public const string stopWord = "RATE US TEN";
        public const string message = "SZLD YQ WFF CFZNFC";
        public const int ALPHABET_LENGTH = 26;
    }
}
