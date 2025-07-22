using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public static class PlayerLoginHandler
    {
        public static void LoginStartMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("IN YOU WANT START YOUR GAME WRITE YOU NICK");
            Console.WriteLine("NICK CAN CONTAIN NUMBERS AND CHARS");
            Console.ResetColor();
        }

        public static void LoginInputMessage()
        {
            Console.Write("Enter your name: ");
        }

        public static string? LoginInGame()
        {
            return Console.ReadLine();
        }

        public static void LoginMessage()
        {
            LoginStartMessage();
            LoginInputMessage();
        }
    }
}
