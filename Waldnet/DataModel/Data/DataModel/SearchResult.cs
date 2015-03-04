using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldnet.Data.DataModel
{
    public class SearchResult
    {
        public string Date { get; private set; }
        public string URL { get; private set; }
        public string Text { get; private set; }
        public string Plaats { get; private set; }
        public string NewsType { get; private set; }

        public SearchResult(string Date, string URL, string Text, string Plaats, string NewsType)
        {
            this.Date = Date;

            if (this.Date[0] == '0')
            {
                this.Date = this.Date.Substring(1);
            }

            this.URL = URL;
            this.Text = Text;
            this.Plaats = Plaats;

            if (this.Plaats[0] == '(')
            {
                this.Plaats = this.Plaats.Substring(1, this.Plaats.Length - 2);
            }

            this.NewsType = "["+ NewsType + "]";
        }
    }
}
