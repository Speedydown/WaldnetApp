using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldnet.Data.DataModel
{
    public class NewsItem
    {
        public string DataName { get; private set; }
        public string Datum { get; private set; }
        public string Header { get; private set; }
        public List<string> Content { get; private set; }
        public List<string> NewsImages { get; private set; }
        public List<Reaction> Reactions { get; private set; }
        public string ReactionCount
        {
            get
            {
                return Reactions.Count + " Reacties";
            }
        }

        public NewsItem(string DataName, string Datum, string Header, List<string> Content, List<string> NewsImages, List<Reaction> Reactions)
        {
            this.DataName = DataName;
            this.Datum = Datum;
            this.Header = Header;
            this.Content = Content;
            this.NewsImages = NewsImages;
            this.Reactions = Reactions;
        }
    }
}
