using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaldnetLogic
{
    public sealed class NewsDay
    {
        public string DayName { get; private set; }
        public IList<NewsLink> NewsLinks { get; private set; }

        public NewsDay(string DayName, IList<NewsLink> NewsLinks)
        {
            this.DayName = DayName;
            this.NewsLinks = NewsLinks;
        }
    }
}
