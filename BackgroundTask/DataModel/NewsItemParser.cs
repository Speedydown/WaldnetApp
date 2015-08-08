using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using Windows.Storage;

namespace BackgroundTask
{
    internal static class NewsItemParser
    {
        private static readonly string[] RemoveFilter = new string[] { "<br>", "<br />", "<br/>", "<BR>", "<p>", "<P>", "<em>", "</em>", "<strong>", "</strong>", "<u>", "</u>" };

        public async static Task<NewsItem> ParseNews(string Input)
        {
            bool EnableImages = true;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableImages"))
            {
                EnableImages = (bool)(ApplicationData.Current.LocalSettings.Values["EnableImages"]);
            }

            NewsItem NI = null;

            try
            {
                Input = GetContentSubstring(Input);
                string DataName = GetDataName(Input);
                string Datum = GetDatum(Input);
                string Header = GetHeader(Input);
                List<string> Content = GetContent(Input);
                List<string> NewsImages = new List<string>();

                string ImageURL = GetImageURL(Input);

                if (EnableImages)
                {
                    if (ImageURL == string.Empty)
                    {
                        string Image = GetImageFromArticle(Input);

                        if (Image != string.Empty && Image.Length < 1000)
                        {
                            NewsImages.Add(Image);
                        }
                    }
                    else
                    {
                        NewsImages = await GetImagesFromImagesPage(ImageURL);

                        if (Content.Count == 1)
                        {
                            Content[0] = CleanLeftOverImageURLIfThereIsOneParagraph(Content[0]);
                        }
                    }
                }

                List<Reaction> Reactions = new List<Reaction>();

                string ReactionsURL = GetReactionsURL(Input);

                if (ReactionsURL != string.Empty)
                {
                    Reactions = await GetReactionsFromURL(ReactionsURL);
                }

                for(int i = 0; i < Content.Count; i++)
                {
                    Content[i] = URLParser(Content[i]);
                }

                NI = new NewsItem(DataName, Datum, Header, Content, NewsImages, Reactions);
            }
            catch
            {
                return null;
            }

            return NI;
        }

        public static string GetContentSubstring(string Input)
        {
            return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=artikel>", Input, true));
        }

        private static string GetDataName(string Input)
        {
            return HTMLParserUtil.GetContentAndSubstringInput("<div class=datanaam>", "&nbsp;</div>", Input, out Input);
        }

        private static string GetDatum(string Input)
        {
            int StartPos = Input.IndexOf("<div class=datazelf>");

            if (StartPos == -1)
            {
                throw new Exception("No datazelf div");
            }


            string SubbedInput = Input.Substring(StartPos);
            int Endpos = SubbedInput.IndexOf("</div>");

            return SubbedInput.Substring("<div class=datazelf>".Length, Endpos - "<div class=datazelf>".Length);
        }

        private static string GetHeader(string Input)
        {
            int StartPos = Input.IndexOf("<h1>");

            if (StartPos == -1)
            {
                throw new Exception("No <h1>");
            }


            string SubbedInput = Input.Substring(StartPos);
            int Endpos = SubbedInput.IndexOf("</h1>");

            return SubbedInput.Substring("<h1>".Length, Endpos - "<h1>".Length);
        }

        private static List<string> GetContent(string Input)
        {
            List<string> Paragraphs = new List<string>();

            int EndOFArticleIndex = Input.IndexOf("<div class=artbalk>");

            if (EndOFArticleIndex == -1)
            {
                throw new Exception("No <div class=artbalk>");
            }

            Input = Input.Substring(0, EndOFArticleIndex);

            while (Input.Length > 0)
            {
                int StartPos = Input.ToLower().IndexOf("<p>");

                if (StartPos == -1)
                {
                    break;
                }


                Input = Input.Substring(StartPos);
                int Endpos = Input.ToLower().IndexOf("</p>");

                string Paragraph = Input.Substring("<p>".Length, Endpos - "<p>".Length);

                if (Paragraph != string.Empty)
                {
                    Paragraph = CleanParagraph(Paragraph);
                    Paragraphs.Add(Paragraph);
                }

                Input = Input.Substring(Endpos + "</p>".Length);

            }

            return Paragraphs;
        }

        private static string CleanLeftOverImageURLIfThereIsOneParagraph(string Paragraph)
        {
            try
            {
                return Paragraph.Substring(0, Paragraph.IndexOf("<a"));
            }
            catch
            {
                return Paragraph;
            }
        }

        private static string CleanParagraph(string Input)
        {
            foreach (string s in RemoveFilter)
            {
                Input = Input.Replace(s, "");
            }

            Input = Input.Replace("&nbsp", " ");
            Input = WebUtility.HtmlDecode(Input);

            //linkjes moeten nog uit de text worden gehaald

            if (Input.Contains("</table>"))
            {
                int StartIndex = Input.IndexOf("<table");
                int EndIndex = Input.LastIndexOf("</table>") + "</table>".Length;

                if (StartIndex != -1 && EndIndex != -1)
                {
                    string Temp = Input.Substring(0, StartIndex);

                    try
                    {
                        Input = Temp + Input.Substring(EndIndex);
                    }
                    catch
                    {
                        Input = Temp;
                    }
                }
            }


            return Input;
        }

        private static string GetImageURL(List<string> Paragraphs)
        {
            string ImageUrl = string.Empty;

            foreach (string s in Paragraphs)
            {
                int IndexOfImage = s.IndexOf("/wn/fotonieuws/");

                if (IndexOfImage == -1)
                {
                    continue;
                }


                ImageUrl = "http://www.waldnet.nl" + s.Substring(s.IndexOf("/wn/fotonieuws/"), s.IndexOf("\">") - s.IndexOf("/wn/fotonieuws/"));

                if (Paragraphs.Count > 1)
                {
                    Paragraphs.Remove(s);
                }

                break;
            }

            return ImageUrl;
        }

        private static string GetImageURL(string Input)
        {
            int IndexOfImage = Input.IndexOf("/wn/fotonieuws/");
            int ENdIndexOfimage = Input.IndexOf("\"><img");

            if (IndexOfImage == -1 || ENdIndexOfimage == -1)
            {
                return string.Empty;
            }

            return "http://www.waldnet.nl" + Input.Substring(IndexOfImage, ENdIndexOfimage - IndexOfImage);
        }

        private static string GetImageFromArticle(string Input)
        {
            int IndexOfImage = Input.IndexOf("http://media.waldnet.nl");
            int ENdIndexOfimage = Input.IndexOf("\" width");

            if (IndexOfImage == -1 || ENdIndexOfimage == -1)
            {
                return string.Empty;
            }

            return Input.Substring(IndexOfImage, ENdIndexOfimage - IndexOfImage);
        }

        private static string GetImageFromArticle(List<string> Paragraphs)
        {
            string ImageUrl = string.Empty;

            for(int i = 0; i < Paragraphs.Count; i++)
            {
                int IndexOfImage = Paragraphs[i].IndexOf("http://media.waldnet.nl/");

                if (IndexOfImage == -1)
                {
                    continue;
                }

                string Tempstring = Paragraphs[i].Substring(Paragraphs[i].IndexOf("http://media.waldnet.nl/"));


                ImageUrl = Tempstring.Substring(0, Tempstring.IndexOf("\""));

                if (Paragraphs.Count > 1)
                {
                    Paragraphs.Remove(Paragraphs[i]);
                    break;
                }
                else
                {
                    Paragraphs[i] = Paragraphs[i].Substring(0, Paragraphs[i].IndexOf("<img"));
                }
            }

            return ImageUrl;
        }

        private async static Task<List<string>> GetImagesFromImagesPage(string URL)
        {
            List<string> ImagesList = new List<string>();

            string Input = await DataHandler.GetDataFromURL(URL);

            int StartIndex = Input.IndexOf("<table border=0 cellspacing=0 cellpadding=0>");
            int EndIndex = Input.IndexOf("</TABLE>");

            if (StartIndex == -1 || EndIndex == -1)
            {
                return ImagesList;
            }

            Input = Input.Substring(StartIndex, EndIndex - StartIndex);

            while (Input.Length > 0)
            {
                int StartPos = Input.ToLower().IndexOf("src=");

                if (StartPos == -1)
                {
                    break;
                }


                Input = Input.Substring(StartPos);
                int Endpos = Input.IndexOf(" border=0>");

                if (Endpos == -1)
                {
                    break;
                }

                string Image = Input.Substring("src=".Length, Endpos - "src=".Length);

                if (!Image.StartsWith("http"))
                {
                    Image = "http://waldnet.nl" + Image;
                }

                if (Image != string.Empty)
                {
                    ImagesList.Add(Image);
                }

                Input = Input.Substring(Endpos + " border=0>".Length);
            }

            return ImagesList;
        }

        private static string GetReactionsURL(string Input)
        {
            string ReactionsURL = string.Empty;

            int IndexOfReactionsURL = Input.IndexOf("/nieuwsreacties.php");

            if (IndexOfReactionsURL == -1)
            {
                return string.Empty;
            }

            Input = Input.Substring(IndexOfReactionsURL);


            ReactionsURL = "http://www.waldnet.nl" + Input.Substring(0, Input.IndexOf("\">"));


            return ReactionsURL;
        }

        private async static Task<List<Reaction>> GetReactionsFromURL(string URL)
        {
            List<Reaction> ReactionList = new List<Reaction>();

            string Input = await DataHandler.GetDataFromURL(URL);

            int StartIndex = Input.IndexOf("<b>REACTIES</b>");

            if (StartIndex == -1)
            {
                return ReactionList;
            }

            Input = Input.Substring(StartIndex);

            while (Input.Length > 0)
            {
                int StartOFUsername = Input.IndexOf("<b><u><b>");

                if (StartOFUsername == -1)
                {
                    break;
                }

                Input = Input.Substring(StartOFUsername);
                StartOFUsername = 0;

                int EndOfUserName = Input.IndexOf("</b></td>");

                if (StartOFUsername == -1 || EndOfUserName == -1)
                {
                    break;
                }

                StartOFUsername += "<b><u><b>".Length;
                string Username = Input.Substring(StartOFUsername, EndOfUserName - StartOFUsername);

                Username = Username.Replace("</b></u>", "");


                int StartOFDate = Input.IndexOf("<font size=1>");
                int EndOFDate = Input.IndexOf("</font>");

                if (StartOFDate == -1 || EndOFDate == -1)
                {
                    break;
                }

                StartOFDate += "<font size=1>".Length;
                string Date = Input.Substring(StartOFDate, EndOFDate - StartOFDate);

                Date = Date.Replace("&nbsp;<br>", "\n");

                int StartOFReaction = Input.IndexOf("97%\">");
                int EndOFReaction = Input.IndexOf("<p>");

                if (StartOFReaction == -1 || EndOFReaction == -1)
                {
                    break;
                }

                StartOFReaction += "97%\">".Length;
                string Reaction = Input.Substring(StartOFReaction, EndOFReaction - StartOFReaction);

                
                Reaction = Reaction.Replace("<br />", "");

                try
                {
                    string Tempsubstring = Reaction.Substring(Reaction.Length - 1);

                    if (Tempsubstring == "\n")
                    {
                        Reaction = Reaction.Substring(0, Reaction.Length - 1);
                    }
                }
                catch
                {

                }

                Reaction = WebUtility.HtmlDecode(Reaction);

                ReactionList.Add(new Reaction(Username, Date, Reaction));

                Input = Input.Substring(EndOFReaction + "<p>".Length);
            }

            return ReactionList;
        }

        public static string URLParser(string Input, bool noQuote = false)
        {
            int IndexOfURL = noQuote ? Input.IndexOf("<a href=") : Input.IndexOf("<a href=\"");

            if (IndexOfURL == -1)
            {
                if (!noQuote)
                {
                    return URLParser(Input, true);
                }

                return Input;
            }

            string Start = Input.Substring(0, IndexOfURL);
            Input = Input.Substring(IndexOfURL + (noQuote ? "<a href=".Length : "<a href=\"".Length));

            IndexOfURL = (noQuote ? Input.IndexOf(">") : Input.IndexOf("\">"));

            if (IndexOfURL == -1)
            {
                return Input;
            }

            IndexOfURL += (noQuote ? ">".Length : "\">".Length);

            int EndOFURL = Input.IndexOf("</a>");

            if (EndOFURL == -1)
            {
                return Input;
            }

            string Content = Input.Substring(IndexOfURL, EndOFURL - IndexOfURL);

            string End = Input.Substring(EndOFURL + "</a>".Length);

            return URLParser(Start + Content + End);
        }

        
    }
}
