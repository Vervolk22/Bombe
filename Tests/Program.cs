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

            //Enigma enigma = new Enigma(3, new int[3] { 6, 2, 23 });
            //Console.WriteLine(enigma.encrypt("RATE US TEN PLEASE"));
            //Console.WriteLine(enigma.encrypt("KCXH TV EAW EKBVGN"));

            EnigmaBreaker breaker = new EnigmaBreaker(3, new int[3] { 6, 2, 22 });
            breaker.tryBreak("KCXH TV EAW EKBVGN");
            //Console.WriteLine(breaker.encrypt("KCXH TV EAW EKBVGN"));



            Console.ReadLine();
        }
    }
}
