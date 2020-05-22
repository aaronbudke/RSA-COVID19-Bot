using System.Net;

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

            int latestTitleSearchIndex = newsString.IndexOf("<h1>", latestArticleClassIndex) + 4;
            int latestTitleSearchEndIndex = newsString.IndexOf("</h1>", latestTitleSearchIndex);
            string latestTitle = "\"" + (newsString.Substring(latestTitleSearchIndex, latestTitleSearchEndIndex - latestTitleSearchIndex)).TrimEnd() + "\"";

            return "*Daily Maverick Latest COVID-19 Article:*\n" + latestTitle + "\n" + latestArticleURL;
        }

        public static string TimesScrape()
        {
            //String searching techniques to find required info.
            WebClient timesClient = new WebClient();
            string newsString = timesClient.DownloadString("https://www.timeslive.co.za/news/latest-covid-19-coronavirus-coverage/");
            int feauteredArticleClassIndex = newsString.IndexOf("article-list-item featured");
            int urlSearchIndex = newsString.IndexOf("href=", feauteredArticleClassIndex) + 6;
            int urlSearchEndIndex = newsString.IndexOf("/\"", urlSearchIndex);
            string featuredArticleURL = newsString.Substring(urlSearchIndex, urlSearchEndIndex - urlSearchIndex);

            int titleSearchIndex = newsString.IndexOf("title=", urlSearchEndIndex) + 7;
            int titleSearchEndIndex = newsString.IndexOf("\"", titleSearchIndex);
            string featuredArticleTitle = "\"" +  newsString.Substring(titleSearchIndex, titleSearchEndIndex - titleSearchIndex) + "\"";
            return "*TimesLIVE Top COVID-19 Article:*\n" + featuredArticleTitle + "\n" + "https://www.timeslive.co.za" + featuredArticleURL;
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

            return "*The Citizen Top COVID-19 Article:*\n" + topArticleTitle + "\n" + citizenTopArticleURL;
        }

        public string AllNews()
        {
            string retString = News24Scrape() + "\n\n" + DailyMavScrape() + "\n\n" + TimesScrape() + "\n\n" + CitizenScrape() + "\n\n"
                   + "A check for latest top articles is made at each new /news request.";

            //Removes unwanted special character codes with correct simple punctuation.
            if (retString.Contains("&#8217;")) { retString = retString.Replace("&#8217;", "\'"); }
            if (retString.Contains("\\&#39;")) { retString = retString.Replace("\\&#39;", "\'"); }

            return retString;
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
                if (iterations == 0) { uRLToParse = "http://www.thepresidency.gov.za/press-statements/president-cyril-ramaphosa?page=0"; }
                if (iterations == 1) { uRLToParse = "http://www.thepresidency.gov.za/press-statements/president-cyril-ramaphosa?page=1"; }
                if (iterations == 2) { break; }

                WebClient presidencyClient = new WebClient();
                presidencyString = presidencyClient.DownloadString(uRLToParse);

                presidencyString = presidencyString.Substring(presidencyString.IndexOf("views-columns"));

                if (presidencyString.ToLower().Contains("address"))
                {
                    articleFound = true;
                    articleClassIndex = presidencyString.ToLower().IndexOf("address") - 250;

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

            return "*Manually Confirmed Date:* No date has been added.\n\n"
                         + "*Automated Bot Search:*\n" + "This was the first press statement I could find about the President's next address:\n\n"
                         + "*Title:* " + articleTitle + "\n" + "*Date published:* " + datePublished + ".\n" + "*Short summary:* " + articleSynopsis
                         + "..." + "\n\n" + articleURL;
        }
    }
}
