using System;
using System.Linq;
using System.Diagnostics;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            var getGamePlayer = new GetGamePlayer();
            getGamePlayer.GetLetter();
            Console.ReadKey();
        }
    }
}