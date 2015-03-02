using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Waldnet.Data.DataModel;

namespace Waldnet.Data
{
    internal static class NewsItemParser
    {
        private static readonly string[] RemoveFilter = new string[] { "<br>", "<br />", "<br/>", "<p>", "<P>" };

        public async static Task<NewsItem> ParseNews(string Input)
        {
            NewsItem NI = null;

            try
            {
                Input = GetContentSubstring(Input);
                string DataName = GetDataName(Input);
                string Datum = GetDatum(Input);
                string Header = GetHeader(Input);
                List<string> Content = GetContent(Input);
                string ImageURL = GetImageURL(Content);
                List<string> NewsImages = new List<string>();
                List<Reaction> Reactions = new List<Reaction>();

                if (ImageURL == string.Empty)
                {
                    string Image = GetImageFromArticle(Content);

                    if (Image != string.Empty)
                    {
                        NewsImages.Add(Image);
                    }
                }
                else
                {
                    NewsImages = await GetImagesFromImagesPage(ImageURL);
                }

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
            int StartPos = Input.IndexOf("<div class=data>");

            if (StartPos == -1)
            {
                throw new Exception("No Content div");
            }

            return Input.Substring(StartPos);
        }

        private static string GetDataName(string Input)
        {
            int StartPos = Input.IndexOf("<div class=datanaam>");

            if (StartPos == -1)
            {
                throw new Exception("No Datanaam div");
            }


            string SubbedInput = Input.Substring(StartPos);
            int Endpos = SubbedInput.IndexOf("&nbsp;</div>");

            return SubbedInput.Substring("<div class=datanaam>".Length, Endpos - "<div class=datanaam>".Length);
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

        private static string CleanParagraph(string Input)
        {
            foreach (string s in RemoveFilter)
            {
                Input = Input.Replace(s, "");
            }

            Input = Input.Replace("&nbsp", " ");
            Input = WebUtility.HtmlDecode(Input);

            //linkjes moeten nog uit de text worden gehaald

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
                Paragraphs.Remove(s);
                break;
            }

            return ImageUrl;
        }

        private static string GetImageFromArticle(List<string> Paragraphs)
        {
            string ImageUrl = string.Empty;

            foreach (string s in Paragraphs)
            {
                int IndexOfImage = s.IndexOf("http://media.waldnet.nl/");

                if (IndexOfImage == -1)
                {
                    continue;
                }

                string Tempstring = s.Substring(s.IndexOf("http://media.waldnet.nl/"));


                ImageUrl = Tempstring.Substring(0, Tempstring.IndexOf("\""));

                Paragraphs.Remove(s);
                break;
            }

            return ImageUrl;
        }

        private async static Task<List<string>> GetImagesFromImagesPage(string URL)
        {
            List<string> ImagesList = new List<string>();

            string Input = await DataHandler.Instance.GetDataFromURL(URL);

            int StartIndex = Input.IndexOf("<div class=paginanb>");

            if (StartIndex == -1)
            {
                return ImagesList;
            }

            Input = Input.Substring(StartIndex);

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

                string Image = "http://waldnet.nl" + Input.Substring("src=".Length, Endpos - "src=".Length);

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

            string Input = await DataHandler.Instance.GetDataFromURL(URL);

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

                int StartOFReaction = Input.IndexOf("width: 474px\">");
                int EndOFReaction = Input.IndexOf("<p>");

                if (StartOFReaction == -1 || EndOFReaction == -1)
                {
                    break;
                }

                StartOFReaction += "width: 474px\">".Length;
                string Reaction = Input.Substring(StartOFReaction, EndOFReaction - StartOFReaction);

                Reaction = Reaction.Replace("<br />", "\n");

                ReactionList.Add(new Reaction(Username, Date, Reaction));

                Input = Input.Substring(EndOFReaction + "<p>".Length);
            }



            return ReactionList;
        }

        public static string URLParser(string Input)
        {
            int IndexOfURL = Input.IndexOf("<a href=\"");

            if (IndexOfURL == -1)
            {
                return Input;
            }

            string Start = Input.Substring(0, IndexOfURL);
            Input = Input.Substring(IndexOfURL + "<a href=\"".Length);

            IndexOfURL = Input.IndexOf("\">");

            if (IndexOfURL == -1)
            {
                return Input;
            }

            IndexOfURL += "\">".Length;

            int EndOFURL = Input.IndexOf("</a>");

            if (EndOFURL == -1)
            {
                return Input;
            }

            string Content = Input.Substring(IndexOfURL, EndOFURL - IndexOfURL);

            string End = Input.Substring(EndOFURL + "</a>".Length);

            return Start + Content + End;
        }

        
    }
}
