using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnigmaCryptography;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //Enigma enigma = new Enigma(5, new int[5] { 6, 2, 23, 12, 1 });
            //Console.WriteLine(enigma.encrypt("HAILGITLERHEISALREADYDEAD"));
            //Console.WriteLine(enigma.encrypt("FISUSVKWQHSQFKSUILQLKVZTY"));
            //Console.ReadLine();

            //Enigma enigma = new Enigma(3, new byte[3] { 6, 2, 23 });
            //Console.WriteLine(enigma.encrypt("RATE US TEN PLEASE"));
            //Console.WriteLine(enigma.encrypt("NXIO ET KXP NTXMKQ"));

            //Enigma enigma = new Enigma(5, new byte[5] { 6, 2, 22, 14, 21 });
            //Console.WriteLine(enigma.encrypt("RATE US TEN PLEASE"));
            //Console.WriteLine(enigma.encrypt("VKRO HO HGH ITZEAA"));

            //EnigmaBreaker breaker = new EnigmaBreaker(3, new byte[3] { 6, 1, 23 });
            //EnigmaBreaker breaker = new EnigmaBreaker(5, new byte[5] { 6, 2, 22, 14, 21 });
            EnigmaBreaker breaker = new EnigmaBreaker(5, new byte[5] { 0, 0, 0, 0, 20 });
            //breaker.tryBreak("NXIO ET KXP NTXMKQ");
            breaker.tryBreak("VKRO HO HGH ITZEAA");
            //Console.WriteLine(breaker.encrypt("KCXH TV EAW EKBVGN"));



            Console.ReadLine();
        }
    }
}
