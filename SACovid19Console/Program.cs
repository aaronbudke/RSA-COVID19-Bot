using System;
using Telegram.Bot;
using unirest_net.http;

namespace SACovid19Console
{
    class Stats
    {
        //Fields
        private static int deathsResponseIndex;
        private static int deathsResponseEndIndex;
        private static int casesResponseIndex;
        private static int casesResponseEndIndex;
        private static int recoveredResponseIndex;
        private static int recoveredResponseEndIndex;

        private static string totalDeaths;
        private static string totalCases;
        private static string totalRecovered;
        private static string responseString;

        //Properties

        //Methods
        public static void StatsSetup()
        {
            HttpResponse<string> response = Unirest.get("https://covid-19-coronavirus-statistics.p.rapidapi.com/v1/stats?country=South+Africa")
                .header("X-RapidAPI-Host", "covid-19-coronavirus-statistics.p.rapidapi.com")
                .header("X-RapidAPI-Key", "38624df731msh2a7de3a003c0ea0p18bfa0jsn873ce6f24b0d")
                .asJson<string>();
            responseString = response.Body.ToString();

            string tempSubString = "";

            if (responseString.Contains("confirmed"))
            {
                casesResponseIndex = responseString.IndexOf("confirmed");
                casesResponseEndIndex = responseString.IndexOf(",", casesResponseIndex);
                tempSubString = responseString.Substring(casesResponseIndex, SearchLength(casesResponseIndex, casesResponseEndIndex)); //Creates substring to be searched.
                totalCases = System.Text.RegularExpressions.Regex.Match(tempSubString, @"\d+").Value;
            }

            if (responseString.Contains("deaths"))
            {
                deathsResponseIndex = responseString.IndexOf("deaths");
                deathsResponseEndIndex = responseString.IndexOf(",", deathsResponseIndex);
                tempSubString = responseString.Substring(deathsResponseIndex, SearchLength(deathsResponseIndex, deathsResponseEndIndex)); //Creates substring to be searched.
                totalDeaths = System.Text.RegularExpressions.Regex.Match(tempSubString, @"\d+").Value; //Searches for specified search digits.
            }

            if (responseString.Contains("recovered"))
            {
                recoveredResponseIndex = responseString.IndexOf("recovered");
                recoveredResponseEndIndex = responseString.IndexOf("}", recoveredResponseIndex);
                tempSubString = responseString.Substring(recoveredResponseIndex, SearchLength(recoveredResponseIndex, recoveredResponseEndIndex)); //Creates substring to be searched.
                totalRecovered = System.Text.RegularExpressions.Regex.Match(tempSubString, @"\d+").Value; //Searches for specified search digits.
            }
        }

        public static int SearchLength(int startIndex, int endIndex)
        {
            return endIndex - startIndex;
        }

        public static string CasesStats()
        {
            return "Total confirmed cases: " + totalCases;
        }

        public static string DeathsStats()
        {
            return "Total confirmed deaths: " + totalDeaths;
        }

        public static string RecoveredStats()
        {
            return "Total recovered: " + totalRecovered;
        }

        public static string LockdownStats()
        {
            DateTime localTime = DateTime.Now;
            DateTime lockdownFinishTime = new DateTime(2020, 4, 16, 0, 0, 0);
            System.TimeSpan timeLeft = lockdownFinishTime.Subtract(localTime);
            string lockdownDaysLeft = Convert.ToString(timeLeft.Days);
            string lockdownHoursLeft = Convert.ToString(timeLeft.Hours);
            string lockdownMinutesLeft = Convert.ToString(timeLeft.Minutes);
            return "Lockdown will end on the 16th of April at 00:00. \n"
                              + "Time remaining: " + lockdownDaysLeft + " days, " + lockdownHoursLeft + " hours, "
                              + lockdownMinutesLeft + " minutes.";
        }

        public static string AllStats()
        {
            return CasesStats() + "\n" + DeathsStats() + "\n" + RecoveredStats() + "\n\n" + LockdownStats();
        }
    }

    class Program
    {
        //Telegram bot functionality:
        private static readonly TelegramBotClient TelegramBot = new TelegramBotClient("1298017188:AAGoGkl6y_CdZOnNDRBPVvnH01UTbNtWNRw");

        private static void TelegramBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string replyText;
            string commandList = "Simply click on the command or send the command to the bot exactly as in the list below (including the forward slash).\n" + "\n"
                                + "COMMANDS:" + "\n"
                                + "/allstats - All available statistics for COVID-19 in RSA.\n"
                                + "/cases - Total cases in RSA.\n"
                                + "/deaths - Total deaths in RSA.\n"
                                + "/recovered - Total recovered cases in RSA.\n"
                                + "/lockdown - Lockdown time remaining and extra info.\n"
                                + "/hotline - Details of government COVID-19 hotline as well as government website information.\n"
                                + "/credits - Information on who created the bot program and resources used.\n"
                                + "/help (or any other non-command text) - Brings up list of commands as shown here.";

            
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                //Send all stats.
                if (e.Message.Text == "/allstats")
                {
                    Stats.StatsSetup();
                    replyText = Stats.AllStats();
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /allstats fullfilled.");
                }

                //Send cases stats.
                else if (e.Message.Text == "/cases")
                {
                    Stats.StatsSetup();
                    replyText = Stats.CasesStats();
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /cases fullfilled.");
                }

                //Send deaths stats.
                else if (e.Message.Text == "/deaths")
                {
                    Stats.StatsSetup();
                    replyText = Stats.DeathsStats();
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /deaths fullfilled.");
                }

                //Send recovered stats.
                else if (e.Message.Text == "/recovered")
                {
                    Stats.StatsSetup();
                    replyText = Stats.RecoveredStats();
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /recovered fullfilled.");
                }

                //Send amount of time remaining for lockdown.
                else if (e.Message.Text == "/lockdown")
                {
                    replyText = Stats.LockdownStats();
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /lockdown fullfilled.");
                }

                //Send hotline number and extra information.
                else if (e.Message.Text == "/hotline")
                {
                    replyText = "Message +27 600 123 456 on WhatsApp with the message 'Hi' to receive further assistance and help regarding COVID-19 in RSA.\n"
                              + "Visit the government website for extra information: https://www.gov.za/Coronavirus";
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /hotline fullfilled.");
                }

                //Send credits (creator and API credits etc.)
                else if (e.Message.Text == "/credits")
                {
                    replyText = "This bot was created and is maintained by Aaron Budke, a first year computer science student." + "\n"
                + "For any suggestions or bug reports, please contact Aaron at: aaronbudkeprogrammer@gmail.com." + "\n" + "\n"
                + "Stats used by the bot are compiled by Johns Hopkins University which have been provided by the COVID-19 Coronavirus Statistic API by KishCom on RapidAPI.com." + "\n"
                + "You may gain access to this free API at: https://rapidapi.com/KishCom/api/covid-19-coronavirus-statistics/endpoints";
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /credits fullfilled.");
                }

                //Send help information on list of commands etc.
                else if (e.Message.Text == "/help")
                {
                    replyText = "Look at the list of commands below to have the bot send you the up to date relevant information you are looking for. "
                              + commandList;
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for /help fullfilled.");
                }

                //Send help information on list of commands etc.
                else
                {
                    replyText = "Command not recognised. Please use the commands as shown below:\n \n"
                                + "Look at the list of commands below to have the bot send you the up to date relevant information you are looking for. "
                                + commandList;
                    TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, replyText);
                    Console.WriteLine("Request for non-recognised command fullfilled.");
                }
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
