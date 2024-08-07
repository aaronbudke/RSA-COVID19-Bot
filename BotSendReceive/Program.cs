﻿using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotSendReceive
{
    class Program
    {
        public static string SALockdownStats()
        {
            TimeZoneInfo timezoneSAST = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            //DateTime timeUCT = DateTime.UctNow;
            DateTime timeSAST = DateTime.Now; //TimeZoneInfo.ConvertTimeFromUtc(timeUCT, timezoneSAST);
            DateTime lockdownFinishTime = new DateTime(2020, 5, 1, 0, 0, 0);
            DateTime lockdownStartTime = new DateTime(2020, 3, 27, 0, 0, 0);

            /*
            System.TimeSpan timeLeft = lockdownFinishTime.Subtract(timeSAST);
            string daysLeft = Convert.ToString(timeLeft.Days);
            string hoursLeft = Convert.ToString(timeLeft.Hours);
            string minutesLeft = Convert.ToString(timeLeft.Minutes);
            */

            System.TimeSpan timePast = timeSAST.Subtract(lockdownStartTime);
            string daysPast = Convert.ToString(timePast.Days);
            string hoursPast = Convert.ToString(timePast.Hours);
            string minutesPast = Convert.ToString(timePast.Minutes);

            return "*Lockdown:* " + "We are nationally in \"Level 4\" of Lockdown. Lockdown levels will be handled per muncipality region in the future. It is expected that "
                              + "most regions will be able to move to \"Level 3\" by the end of May except for those regions that still have high "
                              + "infection rates.\n" + "For more information about the various lockdown levels vist: https://www.lockdownbozza.co.za/intro \n\n"
                              + "Time passsed: " + daysPast + " days, " + hoursPast + " hours, " + minutesPast + " minutes.";
        }

        //Telegram Bot API:
        private static readonly TelegramBotClient TelegramBot = new TelegramBotClient("1298017188:AAGoGkl6y_CdZOnNDRBPVvnH01UTbNtWNRw");
        private static int requests = 1;

        private static void TelegramBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string replyText;
            string commandList = "Look at the list of commands below to have me send you the up-to-date relevant information you are looking for. " +
                                "Simply click on the command or send the command to me exactly as in the list below (including the forward slash).\n" + "\n"
                                + "*Commands:*" + "\n"
                                + "/allstats - All available statistics.\n"
                                + "/worldstats - Global COVID-19 stats.\n"
                                + "/rsastats - All stats of COVID-19 in South Africa.\n"
                                + "/news - Sends links to top articles from local news sites.\n"
                                + "/president - When the next President's address will be.\n"
                                + "/lockdown - Lockdown time remaining and extra info.\n"
                                + "/explained - Video and information explaining all things related to COVID-19 from trusted sources.\n"
                                + "/masks - Images and information explaining when and how to use a mask.\n"
                                + "/hotline - Details of government COVID-19 hotline as well as government website information.\n"
                                + "/about - See what my creator's vision and purpose is for me as a bot.\n"
                                + "/bugs - Send bug reports (problems), suggestions or feedback.\n"
                                + "/patchnotes - See latest fixes and updates to commands.\n"
                                + "/credits - Information on who created me and resources used.\n"
                                + "/help (or any other non-command text) - Brings up list of commands as shown here.";

            if (e.Message.Type == MessageType.Text)
            {
                //Send information on bot's purpose.
                if (e.Message.Text == "/about")
                {
                    replyText = "I was initially created with the intention of purely providing COVID-19 stats to those that wanted to get quick"
                              + " and easy access to these stats. However my creator decided to keep adding functionality to me and I quickly"
                              + " became an all-round assistant that can be used to inform and help those during the COVID-19 pandemic in"
                              + " South Africa. All of the stats and news articles that I provide are the most up-to-date when each new command"
                              + " request is made. Stats are provided by various APIs that can be found in /credits\n\n"
                              + "My creator wants to assure everyone that I was not made to add to any panic or fear during this time."
                              + " Instead I am here to help you during this period. To send my creator any bugs (problems), suggestions"
                              + " or feedback please email him with the subject \"COVID-19 Telegram Bot\" at: aaronbudkeprogrammer@gmail.com.";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText);
                }

                /*
                else if (e.Message.Text == "/subscribe")
                {
                    ChatInfo currentChatInfo = new ChatInfo();
                    currentChatInfo.ChatId = e.Message.Chat.Id;

                    if (System.IO.File.ReadAllText("subscribedChatIds.txt").Contains(currentChatInfo.ToString()))
                    {
                        replyText = "You have already been subscribed to receive important messages from me. To unsubscribe, please click /unsubscribe";
                        TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText);
                    }

                    else
                    {
                        //Writes new ChatID to subscribedChatIds.txt
                        using (StreamWriter subscribedChatIds = System.IO.File.AppendText("subscribedChatIds.txt"))
                        {
                            subscribedChatIds.WriteLine(currentChatInfo.ChatId.ToString());
                            subscribedChatIds.Close();
                        }

                        replyText = "You have been subscribed to receive important push notifications from me regarding the COVID-19 pandemic in South Africa. To unsubscribe, please click /unsubscribe";
                        TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText);
                        TelegramBot.SendTextMessageAsync(chatId: 50757118, text: "A user has signed up for push notifications using /subscribe! Reference chat ID of new user: " + e.Message.Chat.Id);
                    }
                }

                else if (e.Message.Text == "/unsubscribe")
                {
                    ChatInfo currentChatInfo = new ChatInfo();
                    currentChatInfo.ChatId = e.Message.Chat.Id;
                    string filePath = "subscribedChatIds.txt";

                    //Creates arr for new subscribedChatIds file.
                    string unsubcribedChatId = currentChatInfo.ChatId.ToString();
                    var oldFileArr = System.IO.File.ReadAllLines(filePath);
                    string[] newFileArr = new string[oldFileArr.Length - 1];

                    for (int x = 0, iterations = 0; x < oldFileArr.Length; x++)
                    {
                        if (oldFileArr[x] != unsubcribedChatId) { newFileArr[iterations] = oldFileArr[iterations]; iterations++; }
                        else { continue; }
                    }

                    //Deletes old subscribedChatIds.txt and rewrites new file.
                    System.IO.File.Delete(filePath);
                    StreamWriter newFile = new StreamWriter("subscribedChatIds.txt");

                    for (int x = 0; x < newFileArr.Length; x++)
                    {
                        newFile.WriteLine(newFileArr[x]);
                    }
                    newFile.Close();

                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Succesfully unsubscribed from receiving important bot messages about the COVID-19 pandemic in South Africa.");
                    TelegramBot.SendTextMessageAsync(chatId: 50757118, text: "A user has unsubscribed for push notifications using /unsubscribe. Reference chat ID of user: " + e.Message.Chat.Id);
                }

                else if (e.Message.Text.Contains("admin-send-push-notification-12345"))
                {
                    replyText = e.Message.Text.Remove(0, 34);

                    //Sends push notification to each use that has subscribed to special push notifications.
                    StreamReader readSubscribedChatIds = new StreamReader("subscribedChatIds.txt");
                    while (!readSubscribedChatIds.EndOfStream)
                    { 
                        TelegramBot.SendTextMessageAsync(chatId: readSubscribedChatIds.ReadLine(), text: replyText, parseMode: ParseMode.Markdown);
                    }
                    readSubscribedChatIds.Close();
                }
                */

                //Send all stats.
                else if (e.Message.Text == "/allstats")
                {
                    TelegramBot.SendChatActionAsync(chatId: e.Message.Chat, chatAction: ChatAction.Typing);
                    using (StreamReader allStatsReader = new StreamReader("DataUpdate/netcoreapp3.1/allStatsCache.txt"))
                    {
                        replyText = allStatsReader.ReadToEnd() + "\n\n" + SALockdownStats();
                        allStatsReader.Close();
                    }
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View detailed South African stats and map", "https://health.hydra.africa/")));
                }

                //Send world COVID-19 stats.
                else if (e.Message.Text == "/worldstats")
                {
                    TelegramBot.SendChatActionAsync(chatId: e.Message.Chat, chatAction: ChatAction.Typing);
                    using (StreamReader worldStatsReader = new StreamReader("DataUpdate/netcoreapp3.1/worldStatsCache.txt"))
                    {
                        replyText = (worldStatsReader.ReadToEnd());
                        worldStatsReader.Close();
                    }
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View extra stats and global map", "https://coronavirus.jhu.edu/map.html")));
                }

                //Send South African COVID-19 stats cache.
                else if (e.Message.Text == "/rsastats")
                {
                    TelegramBot.SendChatActionAsync(chatId: e.Message.Chat, chatAction: ChatAction.Typing);
                    using (StreamReader rsaStatsReader = new StreamReader("DataUpdate/netcoreapp3.1/rsaStatsCache.txt"))
                    {
                        replyText = rsaStatsReader.ReadToEnd() + "\n\n" + SALockdownStats();
                        rsaStatsReader.Close();
                    }
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View detailed South African stats and map", "https://health.hydra.africa/")));
                }

                //Send webscraped news articles cache.
                else if (e.Message.Text == "/news")
                {
                    TelegramBot.SendChatActionAsync(chatId: e.Message.Chat, chatAction: ChatAction.Typing);
                    using (StreamReader newsReader = new StreamReader("DataUpdate/netcoreapp3.1/newsCache.txt"))
                    {
                        replyText = (newsReader.ReadToEnd());
                        newsReader.Close();
                    }
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                }

                //Send amount of time remaining for lockdown.
                else if (e.Message.Text == "/lockdown")
                {
                    replyText = SALockdownStats();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //Send information on what COVID-19 is.
                else if (e.Message.Text == "/explained")
                {
                    replyText = "This highly informative video by renound and trusted source, Kurzgesagt, explains almost everything you might"
                              + " want to know about COVID-19: " + "https://www.youtube.com/watch?v=BtN-goy9VOY" + "\n\n"
                              + "*World Health Organisation website dedicated to all things COVID-19: *\n"
                              + "https://www.who.int/emergencies/diseases/novel-coronavirus-2019 \n\n"
                              + "*South African Government website dedicated to COVID-19:*\n"
                              + "https://sacoronavirus.co.za/";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //Send media group of images that explain when and how to use a mask.
                else if (e.Message.Text == "/masks")
                {
                    replyText = "The South African Government asks that all citizens wear masks when in public. For more information visit the Government press release by clicking the button below.";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "See Government press release", "https://sacoronavirus.co.za/2020/04/21/use-of-cloth-face-masks-by-members-of-the-general-public-in-south-africa-during-the-covid-19-pandemic/")));
                }

                //Send webscraped information on when President's next address will be.
                else if (e.Message.Text == "/president")
                {
                    TelegramBot.SendChatActionAsync(chatId: e.Message.Chat, chatAction: ChatAction.Typing);
                    using (StreamReader presidentReader = new StreamReader("DataUpdate/netcoreapp3.1/presidentCache.txt"))
                    {
                        replyText = (presidentReader.ReadToEnd());
                        presidentReader.Close();
                    }
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, disableWebPagePreview: false);
                }

                //Send hotline number and extra information.
                else if (e.Message.Text == "/hotline")
                {
                    replyText = "Message +27 600 123 456 on WhatsApp with the message 'Hi' to receive further assistance and help regarding COVID-19 in RSA.\n"
                              + "Visit the government website for extra information.";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "Visit government COVID-19 website", "https://sacoronavirus.co.za/")));
                }

                //Send information on how a user can send bug reports.
                else if (e.Message.Text == "/bugs")
                {
                    replyText = "Please send any bugs (problems), suggestions or feedback with the subject \"COVID-19 Telegram Bot\" to: " +
                              "aaronbudkeprogrammer@gmail.com";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText);
                }

                //Send a list of patch notes.
                else if (e.Message.Text == "/patchnotes")
                {
                    replyText = "*22/05:*\n" + "- Added testing totals for SA stats and tests per 100K population.\n"
                               + "- Added TimesLIVE as additional news site in /news.\n"
                               + "- Greatly improved bot response times by utlising better caching.\n"
                               + "- Improved overall bot logic and responses for requesting information from websites that are often unstable.\n\n"
                               + "*12/05:*\n" + "- Bot maintenance completed and moved bot communication to new server home.\n"
                               + "- Switched to new data repo from Data Science for Social Impact research group at UP as old API has been retired.\n\n"
                               + "*22/04:*\n" + "- Added breakdown of COVID-19 stats per province in /allstats and /rsastats.\n\n"
                               + "*08/04:*\n" + "- Updated /president to search for official press releases from the presidency. "
                               + "Updated searching for relevant statements using better searching techniques.\n"
                               + "- Fixed /lockdown timer to display correct time remaining.\n\n"
                               + "*05/04:*\n" + "- Added /about command that gives more info about bot's purpose.\n"
                               + "- Added /explained that gives info on what COVID-19 is.\n\n"
                               + "*03/04:*\n" + "- Fixed an issue where stats commands, /news and /president did not always automatically update.\n"
                               + "- Fixed an API issue where South African stats were not being fetched.\n"
                               + "- Added functionality to inform the user that an API fetching issue has occured and therefore stats could not"
                               + " be fetched.";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //Send credits (creator and API credits etc.)
                else if (e.Message.Text == "/credits")
                {
                    replyText = "I was created and am maintained by Aaron Budke, a first year computer science student." + "\n"
                + "For any suggestions or bug reports, please contact Aaron with the subject \"COVID-19 Telegram Bot\" at: " +
                "aaronbudkeprogrammer@gmail.com." + "\n" + "\n"
                + "Stats used by the bot are compiled by *Johns Hopkins University* which have been provided by the *COVID-19 Coronavirus Statistic API by Andrew Kish*"
                + " and *Coronavirus Monitor by avalon*, both on RapidAPI.com. \n"
                + "South African per province specific stats are compiled by *Data Science for Social Impact Research Group at University of Pretoria.*\n"
                + "Profile art made by *FreePik* on FreePiks.com\n\n"
                + "To gain access to COVID-19 Coronavirus Statistic API go to: https://rapidapi.com/KishCom/api/covid-19-coronavirus-statistics/endpoints \n"
                + "To gain access to Coronavirus Monitor API go to: https://rapidapi.com/astsiatsko/api/coronavirus-monitor/endpoints \n"
                + "To gain access to Data Science for Social Impact Research Group at University of Pretoria COVID-19 data: https://github.com/dsfsi/covid19za \n"
                + "For more vectors like the profile art go to: https://www.freepik.com/free-photos-vectors/design";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                }

                //Send help information on list of commands etc.
                else if (e.Message.Text == "/help")
                {
                    replyText = commandList;
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                else if (e.Message.Text.ToLower() == "hi" || e.Message.Text.ToLower() == "hi."
                        || e.Message.Text.ToLower() == "hi!" || e.Message.Text.ToLower() == "hi "
                        || e.Message.Text == "/start")
                {
                    replyText = "Hi! I am the unofficial RSA COVID-19 Telegram Bot! I am here to be your personal assistant to "
                              + "help you during the COVID-19 pandemic in South Africa. To begin, follow the steps below to send me your first command!\n\n"
                              + commandList;
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //"Secret" command to send how many requests have been made to bot.
                else if (e.Message.Text == "debug-requests")
                {
                    replyText = "Total requests during current session: " + Convert.ToString(requests);
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText);
                }

                //Send help information on list of commands etc.
                else
                {
                    replyText = "Command not recognised. Please use the commands as shown below:\n \n"
                                + commandList;
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }
            }
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
