using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask
{
    public static class SearchResultParser
    {
        public static IList<SearchResult> GetNewsLinksFromSearchResult(string Input)
        {
            List<SearchResult> NewsLinkList = new List<SearchResult>();
            int IndexOfResults = Input.IndexOf("<div class=content><table cellspacing=1 cellpadding=1 border=0>");

            if (IndexOfResults == -1)
            {
                return NewsLinkList;
            }

            Input = Input.Substring(IndexOfResults + "<div class=content><table cellspacing=1 cellpadding=1 border=0>".Length);

            List<string> ContentList = new List<string>();

            while (Input.Length > 0)
            {
                int StartOfResult = Input.IndexOf("<tr><td>");
                int EndOFResult = Input.IndexOf("</td></tr>");

                if (StartOfResult == -1 || EndOFResult == -1)
                {
                    break;
                }

                StartOfResult = StartOfResult + "<tr><td>".Length;

                ContentList.Add(Input.Substring(StartOfResult, EndOFResult - StartOfResult));
                Input = Input.Substring(EndOFResult + "</td></tr>".Length);
            }

            NewsLinkList = ParseSearchContent(ContentList);
            return NewsLinkList;
        }

        private static List<SearchResult> ParseSearchContent(List<string> SearchContent)
        {
            List<SearchResult> NewsLinkList = new List<SearchResult>();

            foreach (string Content in SearchContent)
            {
                int EndOFDate = Content.IndexOf("</td><td align=left><A HREF=\"");

                string Datum = Content.Substring(0, EndOFDate);

                EndOFDate += "</td><td align=left><A HREF=\"".Length;

                string C = Content.Substring(EndOFDate);

                int EndOFLink = C.IndexOf("\">");

                string Link = "http://www.waldnet.nl"+ C.Substring(0, EndOFLink);

                C = C.Substring(EndOFLink + "\">".Length);

                int EndOFText = C.IndexOf("</a> </td><td><font size=1>");

                string Text = C.Substring(0, EndOFText);

                Text = WebUtility.HtmlDecode(Text).Trim();

                C = C.Substring(EndOFText + "</a> </td><td><font size=1>".Length);

                int EndOfPlaats = C.IndexOf("</font></td><td>");

                string Plaats = C.Substring(0, EndOfPlaats);

                string NewsType = C.Substring(EndOfPlaats + "</font></td><td>".Length);

                NewsLinkList.Add(new SearchResult(Datum, Link, Text, Plaats, NewsType));
                 
            }

            return NewsLinkList;
        }

    }
}
