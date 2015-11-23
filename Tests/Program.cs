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
            Enigma enigma = new Enigma(5, new int[5] { 6, 2, 23, 12, 1 });
            //Console.WriteLine(enigma.encrypt("HAILGITLERHEISALREADYDEAD"));
            Console.WriteLine(enigma.encrypt("FISUSVKWQHSQFKSUILQLKVZTY"));
            Console.ReadLine();
        }
    }
}
