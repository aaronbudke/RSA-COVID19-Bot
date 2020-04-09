using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SACovid19Console
{
    public class WebScraper
    {
       
        //Methods
        public static string News24Scrape()
        {
            //Uses string searching techniques to find top article info.
            WebClient newsClient = new WebClient();
            string newsString = newsClient.DownloadString("https://www.news24.com/SouthAfrica/coronavirus");
            int topArticleClassIndex;
            topArticleClassIndex = newsString.IndexOf("<div class=\"main_story relative\"");
            int topArticleSearchIndex = newsString.IndexOf("href=", topArticleClassIndex) + 6;
            int topArticleSearchEndIndex = newsString.IndexOf("\"", topArticleSearchIndex);
            string topArticleURL = newsString.Substring(topArticleSearchIndex, topArticleSearchEndIndex - topArticleSearchIndex);

            int topTitleSearchIndex = newsString.IndexOf("topstory-", topArticleClassIndex) + 9;
            int topTitleSearchEndIndex = newsString.IndexOf("\"", topTitleSearchIndex);
            string topArticleTitle = "\"" + (newsString.Substring(topTitleSearchIndex, topTitleSearchEndIndex - topTitleSearchIndex)).TrimEnd() + "\"";
            if (topArticleTitle.Contains("\\&#39;")) {  topArticleTitle = topArticleTitle.Replace("\\&#39;", "\'"); }
            return "*News24 Top COVID-19 Article:*\n" + topArticleTitle + "\n" + topArticleURL;
        }

        public static string DailyMavScrape()
        {
            //String searching techniques to find required info.
            WebClient dailyMavClient = new WebClient();
            string newsString = dailyMavClient.DownloadString("https://www.dailymaverick.co.za/article_tag/covid-19/");
            int latestArticleClassIndex;
            latestArticleClassIndex = newsString.IndexOf("media-item") + 10;
            int latestArticleSearchIndex = newsString.IndexOf("href=", latestArticleClassIndex) + 6;
            int latestArticleSearchEndIndex = newsString.IndexOf("/\"", latestArticleSearchIndex);
            string latestArticleURL = newsString.Substring(latestArticleSearchIndex, latestArticleSearchEndIndex - latestArticleSearchIndex);

            int latestTitleSearchIndex =  newsString.IndexOf("<h1>", latestArticleClassIndex) + 4;
            int latestTitleSearchEndIndex = newsString.IndexOf("</h1>", latestTitleSearchIndex);
            string latestTitle = "\"" + (newsString.Substring(latestTitleSearchIndex, latestTitleSearchEndIndex - latestTitleSearchIndex)).TrimEnd() + "\"";

            return "*Daily Maverick Latest COVID-19 Article:*\n" + latestTitle + "\n" + latestArticleURL;
        }

        public static string CitizenScrape()
        {
            //String searching techniques to find relevant info.
            WebClient citizenClient = new WebClient();
            string newsString = citizenClient.DownloadString("https://citizen.co.za/category/news/covid-19/");
            int topArticleClassIndex;
            topArticleClassIndex = newsString.IndexOf("covid-19-breaking-news-tagged") + 26;
            int topArticleSearchIndex = newsString.IndexOf("href=", topArticleClassIndex) + 6;
            int topArticleSearchEndIndex = newsString.IndexOf("\"", topArticleSearchIndex);
            string citizenTopArticleURL = newsString.Substring(topArticleSearchIndex, topArticleSearchEndIndex - topArticleSearchIndex);

            int topArticleTitleSearchIndex = newsString.IndexOf("title=\"Link to ", topArticleClassIndex) + 15;
            int topArticleTitleSearchEndIndex = newsString.IndexOf("\"", topArticleTitleSearchIndex);
            string topArticleTitle = "\"" + (newsString.Substring(topArticleTitleSearchIndex, topArticleTitleSearchEndIndex - topArticleTitleSearchIndex)).TrimEnd()
                + "\"";
            //Removes any unformatted commas and adds formatted commas:"
            if (topArticleTitle.Contains("&#8217;")) { topArticleTitle = topArticleTitle.Replace("&#8217;", "\'"); }

            return "*The Citizen Top COVID-19 Article:*\n" + topArticleTitle + "\n" + citizenTopArticleURL;
        }

        public string AllNews()
        {
            return News24Scrape() + "\n\n" + DailyMavScrape() + "\n\n" + CitizenScrape() + "\n\n"
                   + "A check for latest top articles is made at each new /news request.";
        }

        public string PresidentWebScrape()
        {
            string articleURL = "";
            string articleTitle = "Could not find a recent press statement relating to the President's next address.";
            string articleSynopsis = "";
            string datePublished = "";
            string uRLToParse = "";
            string presidencyString = "";
            int articleClassIndex;
            int iterations = 0;
            bool articleFound = false;

            while (!articleFound)
            {
                if(iterations == 0) { uRLToParse = "http://www.thepresidency.gov.za/press-statements/president-cyril-ramaphosa?page=0"; }
                if (iterations == 1) { uRLToParse = "http://www.thepresidency.gov.za/press-statements/president-cyril-ramaphosa?page=1"; }
                if (iterations == 2) { break; }

                WebClient presidencyClient = new WebClient();
                presidencyString = presidencyClient.DownloadString(uRLToParse);

                presidencyString = presidencyString.Substring(presidencyString.IndexOf("views-columns"));

                if (presidencyString.ToLower().Contains("address"))
                {
                    articleFound = true;
                    articleClassIndex = presidencyString.ToLower().IndexOf("address") - 100;

                    //Searches for a relevant article URL based off of the previosuly found index of our article.
                    int URLIndex = presidencyString.IndexOf("href=\"", articleClassIndex) + 6;
                    int URLEndIndex = presidencyString.IndexOf("\">", URLIndex);
                    articleURL = "http://www.thepresidency.gov.za" + presidencyString.Substring(URLIndex, URLEndIndex - URLIndex);

                    //Searches for title of press release
                    int titleIndex = URLEndIndex + 2;
                    int titleEndIndex = presidencyString.IndexOf("</a>", titleIndex);
                    articleTitle = "\"" + presidencyString.Substring(titleIndex, titleEndIndex - titleIndex).TrimEnd() + "\"";

                    //Finds synopsis of press release
                    int synopsisIndex = presidencyString.IndexOf("field-content\">", titleEndIndex) + 15;
                    int synopsisEndIndex = presidencyString.IndexOf("</span>", synopsisIndex);
                    articleSynopsis = presidencyString.Substring(synopsisIndex, synopsisEndIndex - synopsisIndex);

                    //Finds date published
                    int datePublishedIndex = presidencyString.IndexOf("dateTime", titleEndIndex);
                    datePublishedIndex = presidencyString.IndexOf("\">", datePublishedIndex) + 2; //Refine search for start index.
                    int datePublishedEndIndex = presidencyString.IndexOf("<", datePublishedIndex);
                    datePublished = presidencyString.Substring(datePublishedIndex, datePublishedEndIndex - datePublishedIndex);
                }
                
                iterations++;
            }

            return "*Manually Confirmed Date:* No manually confirmed date has been added yet." + "\n\n" + "*Automated Bot Search:*\n"
                       + "This was the first press statement I could find about the President's next address:\n\n"
                       + "*Title:* " + articleTitle + "\n" + "*Date published:* " + datePublished + "\n" + "*Short summary:* " + articleSynopsis
                       + "..." + "\n\n" + articleURL;
        }
    }
}
