using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputMessageContents;
using System.IO;
using System.Threading;
using TelegramBot.Properties;

namespace TelegramBot
{
    class Program
    {   
        static void Main(string[] args)
        {
            /*
            Task<bool> x;
            do
            {
                x = GetFile();
                do
                {
                    Thread.Sleep(500);
                } while (!x.IsCompleted);
                try { Console.WriteLine(x.Result); }catch { Thread.Sleep(1000); }
                Thread.Sleep(500);
            } while (!x.Result);
            Console.ReadLine();
           */

            Query q = new Query();
            Console.ReadLine();
        }
    }
}
