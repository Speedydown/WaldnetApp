using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaldnetLogic
{
    public sealed class NewsItem
    {
        public string DataName { get; private set; }
        public string Datum { get; private set; }
        public string Header { get; private set; }
        public IList<string> Content { get; private set; }
        public IList<string> NewsImages { get; private set; }
        public IList<Reaction> Reactions { get; private set; }
        public string ReactionCount
        {
            get
            {
                return Reactions.Count + " Reacties";
            }
        }

        public NewsItem(string DataName, string Datum, string Header, IList<string> Content, IList<string> NewsImages, IList<Reaction> Reactions)
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
