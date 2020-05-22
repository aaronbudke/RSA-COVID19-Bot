using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotSendReceive
{
    class Program
    {
        //Telegram Bot API:
        private static readonly TelegramBotClient TelegramBot = new TelegramBotClient("1298017188:AAGoGkl6y_CdZOnNDRBPVvnH01UTbNtWNRw");
        private static int requests = 1;

        private static void TelegramBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
            { 
                requests++;
            }
        
        static void Main(string[] args)
        {
            TelegramBot.OnMessage += TelegramBot_OnMessage;
            TelegramBot.OnMessageEdited += TelegramBot_OnMessage;

            TelegramBot.StartReceiving();
            Console.ReadLine();
            TelegramBot.StopReceiving();
        }
    }
}
