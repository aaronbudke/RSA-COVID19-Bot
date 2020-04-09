using System;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using unirest_net.http;
using Newtonsoft.Json.Linq;

namespace SACovid19Console
{
    class Stats
    {
        //Fields
        private static NumberFormatInfo statsFormat = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 }; 
                                                    //Number format to be used for unformated strings.

        //Methods
        public static string SAStats()
        {
            //Required variables:
            string totalDeaths;
            string totalCases;
            string totalRecovered;
            int totalActiveCasesInt;
            string totalActiveCasesString;

            string responseString;


        //Method makes API request call then uses string searching techniques to find relevant data for each category of stats.
        HttpResponse<string> response = Unirest.get("https://covid-19-coronavirus-statistics.p.rapidapi.com/v1/stats?country=South+Africa")
                                          .header("X-RapidAPI-Host", "covid-19-coronavirus-statistics.p.rapidapi.com")
                                          .header("X-RapidAPI-Key", "38624df731msh2a7de3a003c0ea0p18bfa0jsn873ce6f24b0d")
                                          .asJson<string>();
            responseString = response.Body.ToString();

            if (!responseString.Contains("Bad Gateway") && !responseString.Contains("Country not found"))
            {
                JObject responseJObject = JObject.Parse(responseString);
                int totalCasesInt = 0, totalDeathsInt = 0, totalRecoveredInt = 0;

                if (responseString.Contains("confirmed"))
                {
                    totalCases = (string)responseJObject["data"]["covid19Stats"][0]["confirmed"];
                    totalCasesInt = Convert.ToInt32(totalCases);
                    
                    if (totalCasesInt > 0)
                    {
                        totalCases = totalCasesInt.ToString("n", statsFormat);
                    }
                    else
                    {
                        totalCases = "Could not fetch data.";
                    }
                }
                else { totalCases = "Could not fetch data."; }

                if (responseString.Contains("deaths"))
                {
                    totalDeaths = (string)responseJObject["data"]["covid19Stats"][0]["deaths"];
                    totalDeathsInt = Convert.ToInt32(totalDeaths);

                    if (totalDeathsInt > 0)
                    {
                        totalDeaths = totalDeathsInt.ToString("n", statsFormat);
                    }
                    else
                    {
                        totalDeaths = "Could not fetch data.";
                    }
                }
                else { totalDeaths = "Could not fetch data."; }

                if (responseString.Contains("recovered"))
                {
                    totalRecovered = (string)responseJObject["data"]["covid19Stats"][0]["recovered"];
                    totalRecoveredInt = Convert.ToInt32(totalRecovered);

                    if (totalRecoveredInt > 0)
                    {
                        totalRecovered = totalRecoveredInt.ToString("n", statsFormat);
                        totalActiveCasesInt = totalCasesInt - (totalDeathsInt + totalRecoveredInt);
                        totalActiveCasesString = totalActiveCasesInt.ToString("n", statsFormat);
                    }
                    else
                    {
                        totalRecovered = "Could not fetch data.";
                        totalActiveCasesString = "Could not fetch data.";
                    }
                }
                else 
                { 
                    totalRecovered = "Could not fetch data.";
                    totalActiveCasesString = "Could not fetch data.";
                }

            }
            else
            {
                totalCases = "Could not fetch data.";
                totalDeaths = "Could not fetch data.";
                totalRecovered = "Could not fetch data.";
                totalActiveCasesString = "Could not fetch data";
            }
            return  "*South African COVID-19 Stats:*\n" +
                    "Total confirmed cases: " + totalCases + "\n" + "Total confirmed deaths: " + totalDeaths + "\n" +
                    "Total confirmed recovered: " + totalRecovered + "\n" + "Total active cases: " +  totalActiveCasesString;
        }

        public static string WorldStats()
        {
            //Makes API request to world stats API and then searches for relevant stats in the various categories.
            HttpResponse<string> worldResponse = Unirest.get("https://coronavirus-monitor.p.rapidapi.com/coronavirus/worldstat.php")
                                                .header("X-RapidAPI-Host", "coronavirus-monitor.p.rapidapi.com")
                                                .header("X-RapidAPI-Key", "38624df731msh2a7de3a003c0ea0p18bfa0jsn873ce6f24b0d")
                                                .asJson<string>();

            string totalDeaths;
            string totalCases;
            string totalRecovered;
            int activeCasesInt;
            string activeCasesString;
            string responseString;

            responseString = worldResponse.Body.ToString();
            JObject responseJObject = JObject.Parse(responseString);

            if(responseString.Contains("total_cases"))
            {
                totalCases = ((string)responseJObject["total_cases"]).Replace(",", " ");
            }
            else { totalCases = "Could not fetch data.";  }

            if (responseString.Contains("total_deaths"))
            {
                totalDeaths = ((string)responseJObject["total_deaths"]).Replace(",", " ");
            }
            else { totalDeaths = "Could not fetch data.";  }

            if (responseString.Contains("total_recovered"))
            {
                totalRecovered = ((string)responseJObject["total_recovered"]).Replace(",", " ");
                activeCasesInt = Convert.ToInt32(totalCases.Replace(" ", "")) - (Convert.ToInt32(totalDeaths.Replace(" ", "")) + Convert.ToInt32(totalRecovered.Replace(" ", "")));
                activeCasesString = activeCasesInt.ToString("n", statsFormat);
            }
            else 
            { 
                totalRecovered = "Could not fetch data.";
                activeCasesString = "Could not fetch data.";               
            }

            return "*Global COVID-19 Stats:*\n" + "Global confirmed cases: " + totalCases + "\n" + "Global confirmed deaths: " + totalDeaths + "\n" +
                    "Global confirmed recovered: " + totalRecovered + "\n" + "Global active cases: " + activeCasesString;         
            }

        public static string SALockdownStats()
        {
            TimeZoneInfo timezoneSAST = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
            DateTime timeUCT = DateTime.UtcNow;
            DateTime timeSAST = TimeZoneInfo.ConvertTimeFromUtc(timeUCT, timezoneSAST);
            DateTime lockdownFinishTime = new DateTime(2020, 4, 30, 0, 0, 0);

            System.TimeSpan timeLeft = lockdownFinishTime.Subtract(timeSAST);
            string lockdownDaysLeft = Convert.ToString(timeLeft.Days);
            string lockdownHoursLeft = Convert.ToString(timeLeft.Hours);
            string lockdownMinutesLeft = Convert.ToString(timeLeft.Minutes);
            return  "*Lockdown:* On the 9th of April President Ramaphosa announced that the national lockdown would be extended by two weeks. "
                    + "South African Lockdown will end on the 30th of April at 00:00. \n"
                              + "Time remaining: " + lockdownDaysLeft + " days, " + lockdownHoursLeft + " hours, "
                              + lockdownMinutesLeft + " minutes.";
        }

        public static string AllStats()
        {
            return SAStats() + "\n\n" + WorldStats();
        }
    }

    class Program
    {
        //Telegram Bot API:
        private static readonly TelegramBotClient TelegramBot = new TelegramBotClient("1298017188:AAGoGkl6y_CdZOnNDRBPVvnH01UTbNtWNRw");
        private static int requests = 1;

        private static void TelegramBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string replyText;
            string commandList = "Look at the list of commands below to have me send you the up-to-date relevant information you are looking for. " +
                                "Simply click on the command or send the command to me exactly as in the list below (including the forward slash).\n" + "\n"
                                + "*Commands:*" + "\n"
                                + "/about - See what my creator's vision and purpose is for me as a bot.\n"
                                + "/allstats - All available statistics.\n"
                                + "/worldstats - Global COVID-19 stats.\n"
                                + "/rsastats - All stats of COVID-19 in South Africa.\n"
                                + "/news - Sends links to top articles from local news sites.\n"
                                + "/president - (UPDATED) When the next President's address will be.\n"
                                + "/lockdown - Lockdown time remaining and extra info.\n"
                                + "/explained - (NEW) Video and information explaining all things related to COVID-19 from trusted sources.\n"
                                + "/masks - Images and information explaining when and how to use a mask.\n"
                                + "/hotline - Details of government COVID-19 hotline as well as government website information.\n"
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

                //Send all stats.
                else if (e.Message.Text == "/allstats")
                {
                    replyText = Stats.WorldStats() + "\n\n" + Stats.SAStats() + "\n\n" + Stats.SALockdownStats();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View extra stats and map", "https://coronavirus.jhu.edu/map.html")));
                }

                //Send world COVID-19 stats.
                else if (e.Message.Text == "/worldstats")
                {
                    replyText = Stats.WorldStats();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View extra stats and map", "https://coronavirus.jhu.edu/map.html")));
                }

                //Send South African COVID-19 stats.
                else if (e.Message.Text == "/rsastats")
                {
                    replyText = Stats.SAStats();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "View extra stats and map", "https://coronavirus.jhu.edu/map.html")));
                }

                //Send webscraped new news articles.
                else if (e.Message.Text == "/news")
                {
                    WebScraper currentWebScrape = new WebScraper();
                    replyText = currentWebScrape.AllNews();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
                }

                //Send amount of time remaining for lockdown.
                else if (e.Message.Text == "/lockdown")
                {
                    replyText = Stats.SALockdownStats();
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //Send information on what COVID-19 is.
                else if (e.Message.Text == "/explained")
                {
                    replyText = "*This highly informative video by renound and trusted source, Kurzgesagt, explains almost everything you might"
                              + " want to know about COVID-19: *" + "https://www.youtube.com/watch?v=BtN-goy9VOY" + "\n\n"
                              + " *World Health Organisation website dedicated to all things COVID-19: *\n"
                              + "https://www.who.int/emergencies/diseases/novel-coronavirus-2019 \n\n"
                              + "* South African Government website dedicated to COVID-19:*\n"
                              + "https://sacoronavirus.co.za/";
                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: replyText, parseMode: ParseMode.Markdown);
                }

                //Send media group of images that explain when and how to use a mask.
                else if (e.Message.Text == "/masks")
                {
                    TelegramBot.SendMediaGroupAsync(chatId: e.Message.Chat, inputMedia: new[] {
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-1.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-2.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-3.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-4.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-5.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-6.png"),
                                                    new InputMediaPhoto("https://www.who.int/images/default-source/health-topics/coronavirus/masks/masks-7.png")});

                    TelegramBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Images provided by _The World Health Organisation_ (WHO). Click button below for extra info.",
                                                     replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                                                     "Extra information from WHO",
                                                     "https://www.who.int/emergencies/diseases/novel-coronavirus-2019/advice-for-public/when-and-how-to-use-masks")),
                                                     parseMode: ParseMode.Markdown);
                }

                //Send webscraped information on when President's next address will be.
                else if (e.Message.Text == "/president")
                {
                    WebScraper currentWebScrape = new WebScraper();
                    replyText = currentWebScrape.PresidentWebScrape();
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
                    replyText = "*08/04:*\n" + "- Updated /president to search for official press releases from the presidency. "
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
                + "Profile art made by *FreePik* on FreePiks.com\n\n"
                + "To gain access to COVID-19 Coronavirus Statistic API go to: https://rapidapi.com/KishCom/api/covid-19-coronavirus-statistics/endpoints \n"
                + "To gain access to Coronavirus Monitor API go to: https://rapidapi.com/astsiatsko/api/coronavirus-monitor/endpoints \n"
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
                    replyText = "Hi there! I am the unofficial RSA COVID-19 Telegram Bot! I am here to be your personal assistant to "
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

                requests++;
            }
        }

        static void Main(string[] args)
        {
            //TELEGRAM BOT
            TelegramBot.OnMessage += TelegramBot_OnMessage;
            TelegramBot.OnMessageEdited += TelegramBot_OnMessage;

            TelegramBot.StartReceiving();
            Console.ReadLine();
            TelegramBot.StopReceiving();  
        }
    }
}
