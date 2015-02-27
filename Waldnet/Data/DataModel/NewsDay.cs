using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldnet.Data.DataModel
{
    public class NewsDay
    {
        public string DayName { get; private set; }
        public List<NewsLink> NewsLinks { get; private set; }

        public NewsDay(string DayName, List<NewsLink> NewsLinks)
        {
            this.DayName = DayName;
            this.NewsLinks = NewsLinks;
        }
    }
}
