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
    class User
    {
        public List<List<string>> AllVerbs;
        public List<List<string>> KnownVerbs;

        public readonly long UserID; // ChatID
        public int previousMesasgeID;
        public int currentIt = 0; // итератор для новых глаголов
        public int currentKnownIt = 0; // итератор для итзвестных глаголов
        public User(long ChatID)
        {
            UserID = ChatID;
            KnownVerbs = new List<List<string>>();
            SetVerbs();
        }
        private void SetVerbs()
        {
            StreamReader rd = new StreamReader("C:/Users/Owner/Desktop/Verbs.txt");
            AllVerbs = new List<List<string>>();

            rd.ReadLine();
            for (int i = 0; i < 1000; ++i)
            {
                string wordLine = rd.ReadLine();
                AllVerbs.Add(
                   wordLine.Split('\t').ToList());
            }

            StreamReader newRd = new StreamReader("C:/Users/Owner/Desktop/VerbsUpdated.txt");
            foreach (var el in AllVerbs)
            {
                el.Add(newRd.ReadLine());
            }

        }
        
        public async void SendMainMenu(TelegramBotClient client)
        {
            if (previousMesasgeID != 0) 
            {
                try
                {
                    await client.DeleteMessageAsync(UserID, previousMesasgeID);
                }
                catch (Exception ex) { }
            }
            currentIt = 0;
            currentKnownIt = 0;
            var buttons = new InlineKeyboardButton[][]
                   {
                        new[] { InlineKeyboardButton.WithCallbackData("Я знаю:" + KnownVerbs.Count().ToString(), "MMknownButton"),
                            InlineKeyboardButton.WithCallbackData("Надо повторить:" + AllVerbs.Count().ToString(), "MMrepeatButton") }
                   };

            var keyboard = new InlineKeyboardMarkup(buttons);
            try
            {
                var msg = await client.SendTextMessageAsync(UserID, "Приветствую в приложении для изучения английских глаголов! \n " +
                    "Разработал iSaveliyI", replyMarkup: keyboard);
                previousMesasgeID = msg.MessageId;
            }
            catch (Exception ex) { }
        }
        public async void SendNewVerb(TelegramBotClient client, bool isNeedToTranslate)
        {
            if (currentIt >= AllVerbs.Count() && !isNeedToTranslate)
            {
                SendMainMenu(client);
            }
            else
            {
                var buttons = new InlineKeyboardButton[][]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButton") },
                        new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "button1"), InlineKeyboardButton.WithCallbackData("Надо повторить", "button2") },
                        new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                    };
                var keyboard = new InlineKeyboardMarkup(buttons);
                if (!isNeedToTranslate)
                {
                    await client.DeleteMessageAsync(UserID, previousMesasgeID);
                    try
                    {
                        previousMesasgeID = client.SendTextMessageAsync(UserID,
                            string.Join("\n", AllVerbs[currentIt].Where(_ => AllVerbs[currentIt].IndexOf(_) != AllVerbs[currentIt].Count() - 1)) +
                            "  " + (currentIt + 1).ToString() + "/" + AllVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;
                    }
                    catch (Exception ex) { SendMainMenu(client); }
                    currentIt += 1;
                }
                else
                {
                    var newButtons = new InlineKeyboardButton[][]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "button1"), InlineKeyboardButton.WithCallbackData("Надо повторить", "button2") },
                        new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                    };

                    var newKeyboard = new InlineKeyboardMarkup(newButtons);
                    try
                    {
                        await client.EditMessageTextAsync(UserID, previousMesasgeID,
                            string.Join("\n", AllVerbs[currentIt - 1].Select(_ => AllVerbs[currentIt - 1].IndexOf(_) == AllVerbs[currentIt - 1].Count() - 1 ? _.ToUpper() : _)) +
                            "  " + (currentIt - 1).ToString() + "/" + AllVerbs.Count().ToString(), replyMarkup: newKeyboard);
                    }
                    catch (Exception ex) { SendMainMenu(client); }
                }
            }
        }
        public async void SendKnownVerb(TelegramBotClient client, bool isNeedToTranslate)
        {
            if (currentKnownIt >= KnownVerbs.Count() && !isNeedToTranslate)
            {
                SendMainMenu(client);
            }
            else
            {
                var buttons = new InlineKeyboardButton[][]
                   {
                    new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButtonForKnownWord") },
                    new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "knownButton"), InlineKeyboardButton.WithCallbackData("Надо повторить", "buttonForReRepeat") },
                    new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                   };
                var keyboard = new InlineKeyboardMarkup(buttons);
                if (!isNeedToTranslate)
                {
                    await client.DeleteMessageAsync(UserID, previousMesasgeID);
                    try
                    {
                        previousMesasgeID = client.SendTextMessageAsync(UserID,
                            string.Join("\n", KnownVerbs[currentKnownIt].Where(_ => KnownVerbs[currentKnownIt].IndexOf(_) != KnownVerbs[currentKnownIt].Count() - 1)) +
                            "  " + (currentKnownIt + 1).ToString() + "/" + KnownVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;
                    }
                    catch (Exception ex) { SendMainMenu(client); }

                    currentKnownIt += 1;
                }
                else
                {
                    var newButtons = new InlineKeyboardButton[][]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "knownButton"), InlineKeyboardButton.WithCallbackData("Надо повторить", "buttonForReRepeat") },
                        new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                    };

                    var newKeyboard = new InlineKeyboardMarkup(newButtons);
                    try
                    {
                        await client.EditMessageTextAsync(UserID, previousMesasgeID,
                            string.Join("\n", KnownVerbs[currentKnownIt - 1].Select(_ => KnownVerbs[currentKnownIt - 1].IndexOf(_) ==
                            KnownVerbs[currentKnownIt - 1].Count() - 1 ? _.ToUpper() : _)) +
                            "  " + (currentKnownIt - 1).ToString() + "/" + KnownVerbs.Count().ToString(), replyMarkup: newKeyboard);
                    }
                    catch (Exception ex) { SendMainMenu(client); }
                }
            }
        }

        public async void Button1OnClick(TelegramBotClient client)
        {
            await client.DeleteMessageAsync(UserID, previousMesasgeID);
            if (currentIt >= AllVerbs.Count())
            {
                SendMainMenu(client);
            }
            else
            {
                var buttons = new InlineKeyboardButton[][]
                    {
                    new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButton") },
                    new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "button1"), InlineKeyboardButton.WithCallbackData("Надо повторить", "button2") },
                    new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                    };
                var keyboard = new InlineKeyboardMarkup(buttons);

                currentIt -= 1;
                KnownVerbs.Add(AllVerbs[currentIt]);
                AllVerbs.RemoveAt(currentIt);

                previousMesasgeID = client.SendTextMessageAsync(UserID,
                    string.Join("\n", AllVerbs[currentIt].Where(_ => AllVerbs[currentIt].IndexOf(_) != AllVerbs[currentIt].Count() - 1)) +
                    "  " + (currentIt + 1).ToString() + "/" + AllVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;

                currentIt += 1;
            }
        }
        public async void Button2OnClick(TelegramBotClient client)
        {
            await client.DeleteMessageAsync(UserID, previousMesasgeID);
            if (currentIt >= AllVerbs.Count())
            {
                SendMainMenu(client);
            }
            else
            {
                var buttons = new InlineKeyboardButton[][]
                    {
                    new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButton") },
                    new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "button1"), InlineKeyboardButton.WithCallbackData("Надо повторить", "button2") },
                    new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                    };
                var keyboard = new InlineKeyboardMarkup(buttons);
                previousMesasgeID = client.SendTextMessageAsync(UserID,
                    string.Join("\n", AllVerbs[currentIt].Where(_ => AllVerbs[currentIt].IndexOf(_) != AllVerbs[currentIt].Count() - 1)) +
                    "  " + (currentIt + 1).ToString() + "/" + AllVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;

                currentIt += 1;
            }
        }

        public async void KnownButtonOnClick(TelegramBotClient client)
        {
            await client.DeleteMessageAsync(UserID, previousMesasgeID);
            if (currentKnownIt >= KnownVerbs.Count())
            {
                SendMainMenu(client);
            }
            else
            {
                var buttons = new InlineKeyboardButton[][]
                   {
                    new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButtonForKnownWord") },
                    new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "knownButton"), InlineKeyboardButton.WithCallbackData("Надо повторить", "buttonForReRepeat") },
                    new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                   };
                var keyboard = new InlineKeyboardMarkup(buttons);
                try
                {
                    previousMesasgeID = client.SendTextMessageAsync(UserID,
                        string.Join("\n", KnownVerbs[currentKnownIt].Where(_ => KnownVerbs[currentKnownIt].IndexOf(_) != KnownVerbs[currentKnownIt].Count() - 1)) +
                        "  " + (currentKnownIt + 1).ToString() + "/" + KnownVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;
                }
                catch (Exception ex) { SendMainMenu(client); }
                currentKnownIt += 1;

            }
        }
        public async void ButtonForReRepeatOnClick(TelegramBotClient client)
        {
            await client.DeleteMessageAsync(UserID, previousMesasgeID);
            if (currentKnownIt >= KnownVerbs.Count())
            {
                currentKnownIt -= 1;
                AllVerbs.Add(KnownVerbs[currentKnownIt]);
                KnownVerbs.RemoveAt(currentKnownIt);
                SendMainMenu(client);
            }
            else
            {
                currentKnownIt -= 1;
                AllVerbs.Add(KnownVerbs[currentKnownIt]);
                KnownVerbs.RemoveAt(currentKnownIt);
                var buttons = new InlineKeyboardButton[][]
                   {
                    new[] { InlineKeyboardButton.WithCallbackData("Показать перевод", "translateButtonForKnownWord") },
                    new[] { InlineKeyboardButton.WithCallbackData("Я знаю", "knownButton"), InlineKeyboardButton.WithCallbackData("Надо повторить", "buttonForReRepeat") },
                    new[] { InlineKeyboardButton.WithCallbackData("Вернуться в главное меню", "mainMenuButton") }
                   };
                var keyboard = new InlineKeyboardMarkup(buttons);
                try
                {
                    previousMesasgeID = client.SendTextMessageAsync(UserID,
                        string.Join("\n", KnownVerbs[currentKnownIt].Where(_ => KnownVerbs[currentKnownIt].IndexOf(_) != KnownVerbs[currentKnownIt].Count() - 1)) +
                        "  " + (currentKnownIt + 1).ToString() + "/" + KnownVerbs.Count().ToString(), replyMarkup: keyboard).Result.MessageId;
                }
                catch (Exception ex) { SendMainMenu(client); }
                currentKnownIt += 1;
            }
        }
    }
}
