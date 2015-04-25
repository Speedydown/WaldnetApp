using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask
{
    internal static class PageParser
    {
        public static List<NewsDay> ParseRegionalNews(string Input)
        {
            string Name = string.Empty;
            List<NewsDay> NewsDays = new List<NewsDay>();

            Input = FindStartOfNews(Input);

            while (Input.Length > 0)
            {
                int StartIndexOFURL = Input.IndexOf("<br><b>");

                if (StartIndexOFURL == -1)
                {
                    break;
                }

                StartIndexOFURL = StartIndexOFURL + "<br><b>".Length;

                int IndexOfEndOfName = Input.IndexOf("</b><br><br>");

                if (IndexOfEndOfName == -1)
                {
                    break;
                }

                Name = Input.Substring(StartIndexOFURL, IndexOfEndOfName - StartIndexOFURL);
                Input = Input.Substring(Input.IndexOf("</b><br><br>") + "</b><br><br>".Length);

                NewsDays.Add(new NewsDay(Name, ParseDay(Input)));
            }


            return NewsDays;
        }

        private static string FindStartOfNews(string Input)
        {
            int IndexOfStartYnhald = Input.IndexOf("<div class=ynhald>");
            int IndexOfStartContent = Input.IndexOf("<div class=content>");

            if (IndexOfStartYnhald == -1 && IndexOfStartContent == -1)
            {
                throw new Exception();
            }

            int IndexOfStart = (IndexOfStartYnhald != -1 && IndexOfStartYnhald < IndexOfStartContent) ? IndexOfStartYnhald : IndexOfStartContent;

            return Input.Substring(IndexOfStart + "<div class=ynhald>".Length);
        }

        private static List<NewsLink> ParseDay(string Input)
        {
            List<NewsLink> NewsItems = new List<NewsLink>();

            while (Input.Length > 0)
            {
                int StartIndexOFURL = Input.IndexOf("<a href=\"/wn/nieuws");

                int StopAtNewDate = Input.IndexOf("<br><b>");

                if (StartIndexOFURL == -1 || StartIndexOFURL > StopAtNewDate)
                {
                    break;
                }

                Input = Input.Substring(StartIndexOFURL);

                StartIndexOFURL = 0;

                int EndIndexOFURL = Input.IndexOf("</a>");

                if (StartIndexOFURL == -1 || EndIndexOFURL == -1)
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
                int StartIndexOFURL = Input.IndexOf("/wn/nieuws/");

                if (StartIndexOFURL == -1)
                {
                    break;
                }

                int IndexOfEndOfName = Input.IndexOf("</a><br>");

                if (IndexOfEndOfName == -1)
                {
                    break;
                }

                string HREF = Input.Substring(StartIndexOFURL, IndexOfEndOfName - StartIndexOFURL);
                Input = Input.Substring(Input.IndexOf("</a><br>") + "</a><br>".Length);

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
