using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace BackgroundTask
{
    internal static class PageParser
    {
        public static List<NewsDay> ParseRegionalNews(string Source)
        {
            List<NewsDay> NewsDays = new List<NewsDay>();

            Source = FindStartOfNews(Source);

            while (Source.Length > 0)
            {
                try
                {
                    //Generate Newsheader with dayname first, then parse articles
                    string Name = HTMLParserUtil.GetContentAndSubstringInput("<br><b>", "</b><br><br>", Source, out Source);
                    NewsDays.Add(new NewsDay(Name, ParseDay(Source)));
                }
                catch
                {
                    break;
                }
            }

            return NewsDays;
        }

        private static string FindStartOfNews(string Input)
        {
            int IndexOfStartYnhald = HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=ynhald>", Input, true);
            int IndexOfStartContent = HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=content>", Input, true);

            return Input.Substring((IndexOfStartYnhald != -1 && IndexOfStartYnhald < IndexOfStartContent) ? IndexOfStartYnhald : IndexOfStartContent);
        }

        private static List<NewsLink> ParseDay(string Input)
        {
            List<NewsLink> NewsItems = new List<NewsLink>();

            while (Input.Length > 0)
            {
                int StartIndexOFURL = Input.IndexOf("<a href=\"/wn/nieuws");
                int StopAtNewDate = Input.IndexOf("<br><b>");

                //Stops if new day marker is found before a new newsitem
                if (StartIndexOFURL == -1 || StartIndexOFURL > StopAtNewDate)
                {
                    break;
                }

                Input = Input.Substring(StartIndexOFURL);
                StartIndexOFURL = 0;

                int EndIndexOFURL = Input.IndexOf("</a>");

                if (EndIndexOFURL == -1)
                {
                    break;
                }

                try
                {
                    StartIndexOFURL += "<a href=\"".Length;
                    string Content = Input.Substring(StartIndexOFURL, EndIndexOFURL - StartIndexOFURL);

                    Input = Input.Substring(EndIndexOFURL + "</a>".Length);
                    string[] ContentArray = Content.Split('>');

                    if (ContentArray[0].Contains('\"'))
                    {
                        ContentArray[0] = ContentArray[0].Substring(0, ContentArray[0].Length - 1);
                    }

                    NewsItems.Add(new NewsLink(ContentArray[0], ContentArray[1], string.Empty));
                }
                catch (Exception)
                {

                }
            }

            return NewsItems;
        }

        public static List<NewsLink> ParseBusinessNews(string Input)
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            Input = FindStartOfNews(Input);

            while (Input.Length > 0)
            {
                string HREF = string.Empty;

                try
                {
                    HREF = HTMLParserUtil.GetContentAndSubstringInput("/wn/nieuws/", "</a><br>", Input, out Input);
                }
                catch
                {
                    break;
                }

                string[] ContentArray = HREF.Split('>');

                if (ContentArray[0].Contains('\"'))
                {
                    ContentArray[0] = ContentArray[0].Substring(0, ContentArray[0].Length - 1);
                }

                NewsLinks.Add(new NewsLink(ContentArray[0], ContentArray[1], string.Empty));
            }


            return NewsLinks;
        }

    }
}
