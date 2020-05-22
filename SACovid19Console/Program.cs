using System;
using System.Globalization;
using System.Net;
using unirest_net.http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

namespace SACovid19Console
{
    class Stats
    {
        //Fields
        private static NumberFormatInfo statsFormat = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 }; 
                                                    //Number format to be used for unformated strings.

        //Methods
        public static string[] statsArrIterator(string responseString)
        {
            int refIndex = responseString.LastIndexOf("2020,") + 5;
            refIndex = responseString.IndexOf(',', refIndex) + 1;
            int endRefIndex = responseString.IndexOf(',', refIndex);
            int placeHolder = 0;
            string[] totals = new string[11];
            for (int i = 0; i < 11; i++)
            {
                placeHolder = Convert.ToInt32(responseString.Substring(refIndex, endRefIndex - refIndex));
                totals[i] = placeHolder.ToString("n", statsFormat);
                refIndex = responseString.IndexOf(',', endRefIndex) + 1;
                endRefIndex = responseString.IndexOf(',', refIndex);
            }
            return totals;
        }

        public static string SAStats()
        {
            string totalRecovered;
            int totalActiveCasesInt;
            string totalActiveCasesString;

            string totalsResponseString;

            //Downloads string from .csv data file and then places each total into a string array to be used in return statement and calculations.
            WebClient statsClient = new WebClient();
            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_confirmed.csv");
            string[] totalCasesArr = statsArrIterator(totalsResponseString);

            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_deaths.csv");
            string[] totalDeathsArr = statsArrIterator(totalsResponseString);

            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_testing.csv");
            string[] totalTestsArr = statsArrIterator(totalsResponseString);

            //Calculations of how many tests have been performed per 100K population.
            int ecTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[0], NumberStyles.AllowThousands) / 67.12276);
            int fsTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[1], NumberStyles.AllowThousands) / 28.87465);
            int gpTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[2], NumberStyles.AllowThousands) / 151.76115);
            int kznTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[3], NumberStyles.AllowThousands) / 112.89086);
            int lpTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[4], NumberStyles.AllowThousands) / 59.82584);
            int mpTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[5], NumberStyles.AllowThousands) / 45.92187);
            int ncTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[6], NumberStyles.AllowThousands) / 12.63875);
            int nwTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[7], NumberStyles.AllowThousands) / 40.2716);
            int wcTestsPer100K = (int)Math.Round(Double.Parse(totalTestsArr[8], NumberStyles.AllowThousands) / 68.44272);


            //Method makes API request call then uses string searching techniques to find relevant data for each category of stats.
            HttpResponse<string> totalsResponse = Unirest.get("https://covid-19-coronavirus-statistics.p.rapidapi.com/v1/stats?country=South+Africa")
                                              .header("X-RapidAPI-Host", "covid-19-coronavirus-statistics.p.rapidapi.com")
                                              .header("X-RapidAPI-Key", "38624df731msh2a7de3a003c0ea0p18bfa0jsn873ce6f24b0d")
                                              .asJson<string>();
                totalsResponseString = totalsResponse.Body.ToString();

            if (!totalsResponseString.Contains("Bad Gateway") && !totalsResponseString.Contains("Country not found"))
            {
                JObject responseJObject = JObject.Parse(totalsResponseString);
                int totalRecoveredInt = 0;

                if (totalsResponseString.Contains("recovered"))
                {
                    totalRecovered = (string)responseJObject["data"]["covid19Stats"][0]["recovered"];
                    int totalCasesHopkins = Convert.ToInt32((string)responseJObject["data"]["covid19Stats"][0]["confirmed"]);
                    int totalDeathsHopkins = Convert.ToInt32((string)responseJObject["data"]["covid19Stats"][0]["deaths"]);
                    totalRecoveredInt = Convert.ToInt32(totalRecovered);

                    if (totalRecoveredInt > 0)
                    {
                        totalRecovered = totalRecoveredInt.ToString("n", statsFormat);
                        totalActiveCasesInt = totalCasesHopkins - (totalDeathsHopkins + totalRecoveredInt);
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
                totalRecovered = "Could not fetch data.";
                totalActiveCasesString = "Could not fetch data";
            }

            return "*South African COVID-19 Stats:*\n" +
                    "Total confirmed cases: " + totalCasesArr[10] + "\n" + "Total confirmed deaths: " + totalDeathsArr[10] + "\n" +
                    "Total confirmed recovered: " + totalRecovered + "\n" + "Total active cases: " + totalActiveCasesString + "\n" +
                    "Total tests performed: " + totalTestsArr[10] + "\n\n"
                        + "*Stats Per Province:*\n" + "Gauteng:\n" + "Cases: " + totalCasesArr[2] + "\nDeaths: " + totalDeathsArr[2] + "\nTests performed: " + totalTestsArr[2] + "\nTests per 100K population: " + gpTestsPer100K
                        + "\n\nWestern Cape:\n" + "Cases: " + totalCasesArr[8] + "\nDeaths: " + totalDeathsArr[8] + "\nTests performed: " + totalTestsArr[8] + "\nTests per 100K population: " + wcTestsPer100K
                        + "\n\nKwaZulu-Natal:\n" + "Cases: " + totalCasesArr[3] + "\nDeaths: " + totalDeathsArr[3] + "\nTests performed: " + totalTestsArr[3] + "\nTests per 100K population: " + kznTestsPer100K
                        + "\n\nFree State:\n" + "Cases: " + totalCasesArr[1] + "\nDeaths: " + totalDeathsArr[1] + "\nTests performed: " + totalTestsArr[1] + "\nTests per 100K population: " + fsTestsPer100K
                        + "\n\nEastern Cape:\n" + "Cases: " + totalCasesArr[0] + "\nDeaths: " + totalDeathsArr[0] + "\nTests performed: " + totalTestsArr[0] + "\nTests per 100K population: " + ecTestsPer100K
                        + "\n\nNorthern Cape:\n" + "Cases: " + totalCasesArr[6] + "\nDeaths: " + totalDeathsArr[6] + "\nTests performed: " + totalTestsArr[6] + "\nTests per 100K population: " + ncTestsPer100K
                        + "\n\nNorth West:\n" + "Cases: " + totalCasesArr[7] + "\nDeaths: " + totalDeathsArr[7] + "\nTests performed: " + totalTestsArr[7] + "\nTests per 100K population: " + nwTestsPer100K
                        + "\n\nLimpopo:\n" + "Cases: " + totalCasesArr[4] + "\nDeaths: " + totalDeathsArr[4] + "\nTests performed: " + totalTestsArr[4] + "\nTests per 100K population: " + lpTestsPer100K
                        + "\n\nMpumulanga:\n" + "Cases: " + totalCasesArr[5] + "\nDeaths: " + totalDeathsArr[5] + "\nTests performed: " + totalTestsArr[5] + "\nTests per 100K population: " + mpTestsPer100K
                        + "\n\nUnknown:\n" + "Cases: " + totalCasesArr[9] + "\nDeaths: " + totalDeathsArr[9] + "\nTests performed: " + totalTestsArr[9];
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

            if (responseString.Contains("total_cases"))
            { 
                if (responseString.Contains("total_cases"))
                {
                    totalCases = ((string)responseJObject["total_cases"]).Replace(",", " ");
                }
                else { totalCases = "Could not fetch data."; }

                if (responseString.Contains("total_deaths"))
                {
                    totalDeaths = ((string)responseJObject["total_deaths"]).Replace(",", " ");
                }
                else { totalDeaths = "Could not fetch data."; }

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
            }
            else
            {
                totalCases = "Could not fetch data.";
                totalDeaths = "Could not fetch data.";
                totalRecovered = "Could not fetch data.";
                activeCasesString = "Could not fetch data.";
            }

            return "*Global COVID-19 Stats:*\n" + "Global confirmed cases: " + totalCases + "\n" + "Global confirmed deaths: " + totalDeaths + "\n" +
                    "Global confirmed recovered: " + totalRecovered + "\n" + "Global active cases: " + activeCasesString;         
            }

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

        public static string AllStats()
        {
            return WorldStats() + "\n\n" + SAStats() + "\n\n" + SALockdownStats();
        }
    }

    class ChatInfo
    {
        //FIELDS
        private long chatId;
        
        //PROPERTIES
        public long ChatId { get; set; }        
    }

    class Program
    {
        static void Main(string[] args)
        {
            int x = 0;
            while(x < 2)
            {
                //Updates all web reliant commands and stores relevant text response.
                DateTime updateTime = DateTime.Now;
                string worldStats = Stats.WorldStats();
                string rsaStats = Stats.SAStats();
                string allStats = worldStats + "\n\n" + rsaStats;

                WebScraper currentScrape = new WebScraper();
                string news = currentScrape.AllNews();
                string president = currentScrape.PresidentWebScrape();

                //Writes to allStatsCache file.
                using (StreamWriter allStatsCache = File.CreateText("allStatsCache.txt"))
                {
                    allStatsCache.Write(allStats);
                    allStatsCache.Close();
                }

                //Writes to worldStatsCache file.
                using (StreamWriter worldStatsCache = File.CreateText("worldStatsCache.txt"))
                {
                    worldStatsCache.Write(worldStats);
                    worldStatsCache.Close();
                }

                //Writes to rsaStatsCache file.
                using (StreamWriter rsaStatsCache = File.CreateText("rsaStatsCache.txt"))
                {
                    rsaStatsCache.Write(rsaStats);
                    rsaStatsCache.Close();
                }

                //Writes to newsCache text file.
                using (StreamWriter newsCache = File.CreateText("newsCache.txt"))
                {
                    newsCache.Write(news);
                    newsCache.Close();
                }

                //Writes to presidentCache file.
                using (StreamWriter presidentCache = File.CreateText("presidentCache.txt"))
                {
                    presidentCache.Write(president);
                    presidentCache.Close();
                }

                Thread.Sleep(60000);
            }
        }
    }
}
