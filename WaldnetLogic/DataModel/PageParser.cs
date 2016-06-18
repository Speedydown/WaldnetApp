using BaseLogic.HtmlUtil;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaldnetLogic
{
    internal static class PageParser
    {
        public static List<NewsDay> ParseRegionalNews(string Source)
        {
            List<NewsLink> NewsLinks = new List<NewsLink>();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(Source);

            if (htmlDoc.DocumentNode != null)
            {
                var HeadlineNodes = htmlDoc.DocumentNode.Descendants("div").Where(d => d.Attributes.Count(a => a.Value.Contains("ynhald")) > 0);

                foreach (HtmlNode node in HeadlineNodes)
                {
                    try
                    {
                        int OnclickIndex = node.OuterHtml.IndexOf("/wn/nieuws/");
                        int Length = node.OuterHtml.IndexOf(".html") + ".html".Length - OnclickIndex;

                        string Url = node.OuterHtml.Substring(OnclickIndex, Length);
                        string TimeStamp = node.Descendants("div").Where(d => d.Attributes.Count(a => a.Value.Contains("haadfak")) > 0).FirstOrDefault().Descendants("br").FirstOrDefault().PreviousSibling.InnerText;


                        string Title = node.Descendants("h2").FirstOrDefault().InnerText;

                        NewsLinks.Add(new NewsLink(Url, Title, TimeStamp.Trim()));
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            return OrderNewsLinks(NewsLinks);
        }

        private static List<NewsDay> OrderNewsLinks(List<NewsLink> NewsLinks)
        {
            List<NewsDay> NewsDays = new List<NewsDay>();

            foreach (NewsLink nl in NewsLinks)
            {
                DateTime Date = DateTime.Now;

                if (nl.TimeStamp.ToLower().Contains("uur"))
                {
                    string uurString = nl.TimeStamp.Split(' ').First();

                    int uur = 0;
                    int.TryParse(uurString, out uur);

                    if (uur == 0)
                    {
                        continue;
                    }

                    Date = DateTime.Now.AddHours(-uur);

                   
                }
                else
                {
                    string daystring = nl.TimeStamp.Split(' ')[1];
                    string MonthString = nl.TimeStamp.Split(' ')[2];

                    Date = DateTime.Now;
                    DateTime.TryParse(daystring + " " + MonthString + " " + DateTime.Now.Year, out Date);
                }

                NewsDay CurrentNewsDay = NewsDays.FirstOrDefault(nd => nd.DayName == Date.ToString("dddd dd MMMM"));

                if (CurrentNewsDay == null)
                {
                    NewsDays.Add(new NewsDay(Date.ToString("dddd dd MMMM"), new List<NewsLink>() { nl }));
                }
                else
                {
                    CurrentNewsDay.NewsLinks.Add(nl);
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
