using System;
using System.Globalization;
using System.Net;
using unirest_net.http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using Azure.Storage.Blobs.Models;
using System.Net.Http.Headers;

namespace SACovid19Console
{
    class Stats
    {
        //Fields
        private static NumberFormatInfo statsFormat = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 }; 
                                                    //Number format to be used for unformated strings.

        //Methods
        public static int[] statsArrIterator(string responseString, int day)
        {
            //int day represent how many days back the data must go. 0 = most recent, 1 = day before that etc.
            string[] totalsPerDay = responseString.Split("2020,");
            string totalsStr;

            if (day == 0)
            {
                totalsStr = totalsPerDay[totalsPerDay.Length - 1];
            }
            else if (day == 1)
            {
                totalsStr = totalsPerDay[totalsPerDay.Length - 2];
            }
            else
            {
                totalsStr = totalsPerDay[totalsPerDay.Length - 1];
            }

            int placeHolder;
            int refIndex = totalsStr.IndexOf(",") + 1;
            int endRefIndex = totalsStr.IndexOf(',', refIndex);
            int[] totalsArr = new int[11];
            for (int i = 0; i < 11; i++)
            {
                placeHolder = Convert.ToInt32(totalsStr.Substring(refIndex, endRefIndex - refIndex));
                totalsArr[i] = placeHolder;
                refIndex = totalsStr.IndexOf(',', endRefIndex) + 1;
                endRefIndex = totalsStr.IndexOf(',', refIndex);
            }
            return totalsArr;
        }

        public static string[] StatsArrStrConvert(int[] totalsArr)
        {
            string[] returnStrArr = new string[11];
            
            for (int i = 0; i < 11; i++)
            {
                returnStrArr[i] = totalsArr[i].ToString("n", statsFormat);
            }
            return returnStrArr;
        }

        public static string[] SAStatsIncrease(int[] todayTotals, int[] yesterdayTotals)
        {
            string[] dailyIncrease = new string[11];
            int placeHolderInt;
            string placeHolderStr;
            for (int i = 0; i < 11; i++)
            {
                placeHolderInt = todayTotals[i] - yesterdayTotals[i];
                if (placeHolderInt > 0)
                {
                    placeHolderStr = placeHolderInt.ToString("n", statsFormat);
                    dailyIncrease[i] = "(+" + placeHolderStr + ")";
                }
                else if (placeHolderInt < 0)
                {
                    placeHolderStr = placeHolderInt.ToString("n", statsFormat);
                    dailyIncrease[i] = "(" + placeHolderStr + ")";
                }
                else
                {
                    dailyIncrease[i] = "";
                }
            }
            return dailyIncrease;
        }

        public static string SAStats()
        {
            string totalRecovered;
            int totalActiveCasesInt;
            string totalActiveCasesString;

            string totalsResponseString;

            //Downloads string from .csv data file and then places each total into a int array to be used in return statement and calculations.
            WebClient statsClient = new WebClient();
            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_confirmed.csv");
            int[] casesTotalInt = statsArrIterator(totalsResponseString, 0);
            int[] casesTotalYesterdayInt = statsArrIterator(totalsResponseString, 1);
            string[] totalCasesIncrease = SAStatsIncrease(casesTotalInt, casesTotalYesterdayInt);

            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_deaths.csv");
            int[] deathsTotalInt = statsArrIterator(totalsResponseString, 0);
            int[] deathsTotalYesterdayInt = statsArrIterator(totalsResponseString, 1);
            string[] totalDeathsIncrease = SAStatsIncrease(deathsTotalInt, deathsTotalYesterdayInt);

            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_recoveries.csv");
            int[] recoveredTotalInt = statsArrIterator(totalsResponseString, 0);
            int[] recoveredTotalYesterdayInt = statsArrIterator(totalsResponseString, 1);
            string[] totalRecoveredIncrease = SAStatsIncrease(recoveredTotalInt, recoveredTotalYesterdayInt);

            totalsResponseString = statsClient.DownloadString("https://raw.githubusercontent.com/dsfsi/covid19za/master/data/covid19za_provincial_cumulative_timeline_testing.csv");
            int[] totalTestsInt = statsArrIterator(totalsResponseString, 0);

            //Calculations of how many tests have been performed per 100K population.
            int ecTestsPer100K = (int)Math.Round(totalTestsInt[0] / 67.12276, 0);
            int fsTestsPer100K = (int)Math.Round(totalTestsInt[1] / 28.87465, 0);
            int gpTestsPer100K = (int)Math.Round(totalTestsInt[2] / 151.76115, 0);
            int kznTestsPer100K = (int)Math.Round(totalTestsInt[3] / 112.89086, 0);
            int lpTestsPer100K = (int)Math.Round(totalTestsInt[4] / 59.82584, 0);
            int mpTestsPer100K = (int)Math.Round(totalTestsInt[5] / 45.92187, 0);
            int ncTestsPer100K = (int)Math.Round(totalTestsInt[6] / 12.63875, 0);
            int nwTestsPer100K = (int)Math.Round(totalTestsInt[7] / 40.2716, 0);
            int wcTestsPer100K = (int)Math.Round(totalTestsInt[8] / 68.44272, 0);

            //Calculations of active cases
            int[] activeCasesArr = new int[11];
            for(int i = 0; i < 11; i++)
            {
                int placeHolderInt;
                activeCasesArr[i] = casesTotalInt[i] - deathsTotalInt[i] - recoveredTotalInt[i];
            }

            int[] activeCasesYesterdayArr = new int[11];
            for (int i = 0; i < 11; i++)
            {
                int placeHolderInt;
                activeCasesYesterdayArr[i] = casesTotalYesterdayInt[i] - deathsTotalYesterdayInt[i] - recoveredTotalYesterdayInt[i];
            }

            //Calculation of increase in active cases
            string[] activeCasesIncreaseArr = new string[11];
            for (int i = 0; i < 11; i++)
            {
                int placeHolderInt;
                placeHolderInt = activeCasesArr[i] - activeCasesYesterdayArr[i];
                if (placeHolderInt > 0)
                {
                    activeCasesIncreaseArr[i] = "(+" +  placeHolderInt.ToString("n", statsFormat) + ")";
                }
                else if (placeHolderInt < 0)
                {
                    activeCasesIncreaseArr[i] = "(" + placeHolderInt.ToString("n", statsFormat) + ")";
                }
                else
                {
                    activeCasesIncreaseArr[i] = "";
                }
            }

            //Converts int[] arrays to string arrays with correct stats format to be printed.
            string[] casesTotalStr = StatsArrStrConvert(casesTotalInt);
            string[] deathsTotalStr = StatsArrStrConvert(deathsTotalInt);
            string[] recoveredTotalStr = StatsArrStrConvert(recoveredTotalInt);
            string[] testsTotalStr = StatsArrStrConvert(totalTestsInt);

            return "*South African COVID-19 Stats:*\n" +
                    "Total confirmed cases: " + casesTotalStr[10] + totalCasesIncrease[10] + "\n" +
                    "Total confirmed deaths: " + deathsTotalStr[10] + totalDeathsIncrease[10] + "\n" +
                    "Total confirmed recovered: " + recoveredTotalStr[10] + totalRecoveredIncrease[10] + "\n" + "Total active cases: " + activeCasesArr[10] + activeCasesIncreaseArr[10] +  "\n" +
                    "Total tests performed: " + testsTotalStr[10] + "\n\n"
                        + "*Stats Per Province:*\n" + "Gauteng:\n" + "Cases: " + casesTotalStr[2] + totalCasesIncrease[2] + "\nDeaths: " + deathsTotalStr[2] + totalDeathsIncrease[2] + "\nRecovered: " + recoveredTotalStr[2] + totalRecoveredIncrease[2] 
                        + "\nActive Cases: " + activeCasesArr[2] + activeCasesIncreaseArr[2] + "\nTests performed: " + testsTotalStr[2] + "\nTests per 100K population: " + gpTestsPer100K
                        
                        + "\n\nWestern Cape:\n" + "Cases: " + casesTotalStr[8] + totalCasesIncrease[8] + "\nDeaths: " + deathsTotalStr[8] + totalDeathsIncrease[8] + "\nRecovered: " + recoveredTotalStr[8] + totalRecoveredIncrease[8]
                        + "\nActive Cases: " + activeCasesArr[8] + activeCasesIncreaseArr[8] + "\nTests performed: " + testsTotalStr[8] + "\nTests per 100K population: " + wcTestsPer100K
                        
                        + "\n\nKwaZulu-Natal:\n" + "Cases: " + casesTotalStr[3] + totalCasesIncrease[3] + "\nDeaths: " + deathsTotalStr[3] + totalDeathsIncrease[3] + "\nRecovered: " + recoveredTotalStr[3] + totalRecoveredIncrease[3]
                        + "\nActive Cases: " + activeCasesArr[3] + activeCasesIncreaseArr[3] + "\nTests performed: " + testsTotalStr[3] + "\nTests per 100K population: " + kznTestsPer100K
                        
                        + "\n\nFree State:\n" + "Cases: " + casesTotalStr[1] + totalCasesIncrease[1] + "\nDeaths: " + deathsTotalStr[1] + totalDeathsIncrease[1] + "\nRecovered: " + recoveredTotalStr[1] + totalRecoveredIncrease[1]
                        + "\nActive Cases: " + activeCasesArr[1] + activeCasesIncreaseArr[1] + "\nTests performed: " + testsTotalStr[1] + "\nTests per 100K population: " + fsTestsPer100K
                        
                        + "\n\nEastern Cape:\n" + "Cases: " + casesTotalStr[0] + totalCasesIncrease[0] + "\nDeaths: " + deathsTotalStr[0] + totalDeathsIncrease[0] + "\nRecovered: " + recoveredTotalStr[0] + totalRecoveredIncrease[0]
                        + "\nActive Cases: " + activeCasesArr[0] + activeCasesIncreaseArr[0] + "\nTests performed: " + testsTotalStr[0] + "\nTests per 100K population: " + ecTestsPer100K
                        
                        + "\n\nNorthern Cape:\n" + "Cases: " + casesTotalStr[6] + totalCasesIncrease[6] + "\nDeaths: " + deathsTotalStr[6] + totalDeathsIncrease[6] + "\nRecovered: " + recoveredTotalStr[6] + totalRecoveredIncrease[6]
                        + "\nActive Cases: " + activeCasesArr[6] + activeCasesIncreaseArr[6] + "\nTests performed: " + testsTotalStr[6] + "\nTests per 100K population: " + ncTestsPer100K
                        
                        + "\n\nNorth West:\n" + "Cases: " + casesTotalStr[7] + totalCasesIncrease[7] + "\nDeaths: " + deathsTotalStr[7] + totalDeathsIncrease[7] + "\nRecovered: " + recoveredTotalStr[7] + totalRecoveredIncrease[7]
                        + "\nActive Cases: " + activeCasesArr[7] + activeCasesIncreaseArr[7] + "\nTests performed: " + testsTotalStr[7] + "\nTests per 100K population: " + nwTestsPer100K

                        + "\n\nLimpopo:\n" + "Cases: " + casesTotalStr[4] + totalCasesIncrease[4] + "\nDeaths: " + deathsTotalStr[4] + totalDeathsIncrease[4] + "\nRecovered: " + recoveredTotalStr[4] + totalRecoveredIncrease[4]
                        + "\nActive Cases: " + activeCasesArr[4] + activeCasesIncreaseArr[4] + "\nTests performed: " + testsTotalStr[4] + "\nTests per 100K population: " + lpTestsPer100K

                        + "\n\nMpumulanga:\n" + "Cases: " + casesTotalStr[5] + totalCasesIncrease[5] + "\nDeaths: " + deathsTotalStr[5] + totalDeathsIncrease[5] + "\nRecovered: " + recoveredTotalStr[5] + totalRecoveredIncrease[5]
                        + "\nActive Cases: " + activeCasesArr[5] + activeCasesIncreaseArr[5] + "\nTests performed: " + testsTotalStr[5] + "\nTests per 100K population: " + mpTestsPer100K

                        + "\n\nUnknown:\n" + "Cases: " + casesTotalStr[9] + totalCasesIncrease[9] + "\nDeaths: " + deathsTotalStr[9] + totalDeathsIncrease[9] + "\nRecovered: " + recoveredTotalStr[9] + totalRecoveredIncrease[9]
                        + "\nActive Cases: " + activeCasesArr[9] + activeCasesIncreaseArr[9] + "\nTests performed: " + testsTotalStr[9];
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

                Thread.Sleep(600000);
            }
        }
    }
}
