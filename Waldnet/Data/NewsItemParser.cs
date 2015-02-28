using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldnet.Data.DataModel;

namespace Waldnet.Data
{
    internal static class NewsItemParser
    {
        public static NewsItem ParseNewsURL(string URL)
        {
            string Input = DataHandler.Instance.GetRegionalNews(URL);

            string Datanaam = string.Empty;
            string 
        }
    }
}
