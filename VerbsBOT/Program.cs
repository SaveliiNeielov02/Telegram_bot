using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Linq;
using Telegram.Bot.Types.Enums;

namespace VerbsBOT
{
    class Program
    {
        public static TelegramBotClient client = new TelegramBotClient((string)null);
        public static List<User> users = new List<User>();
        static void Main(string[] args)
        {
            client.StartReceiving(OnUpdate, Error);
            Console.ReadLine();
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private async static Task OnUpdate(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {

            var message = update.Message;
            var queryMsg = update.CallbackQuery;
            if (message != null)
            {
                if (message.Text == "/start")
                {
                    if (users.Find(user => user.UserID == message.Chat.Id) == null) 
                    {
                        users.Add(new User(message.Chat.Id));
                    }
                    users.Find(_ => _.UserID == message.Chat.Id).SendMainMenu(client);
                }
            }
            if (queryMsg != null)
            {
                if (users.Count() > 0)
                {
                    if (update.CallbackQuery.Data == "MMrepeatButton")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).SendNewVerb(client, isNeedToTranslate: false);
                    }
                    else if (update.CallbackQuery.Data == "MMknownButton")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).SendKnownVerb(client, isNeedToTranslate: false);
                    }
                    else if (update.CallbackQuery.Data == "button1")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).Button1OnClick(client);
                    }
                    else if (update.CallbackQuery.Data == "button2")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).Button2OnClick(client);
                    }
                    else if (update.CallbackQuery.Data == "translateButton")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).SendNewVerb(client, isNeedToTranslate: true);
                    }
                    else if (update.CallbackQuery.Data == "mainMenuButton")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).SendMainMenu(client);
                    }
                    else if (update.CallbackQuery.Data == "translateButtonForKnownWord")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).SendKnownVerb(client, isNeedToTranslate: true);
                    }
                    else if (update.CallbackQuery.Data == "knownButton")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).KnownButtonOnClick(client);
                    }
                    else if (update.CallbackQuery.Data == "buttonForReRepeat")
                    {
                        users.Find(_ => _.UserID == queryMsg.From.Id).ButtonForReRepeatOnClick(client);
                    }
                }
            }
        }
    }
}
