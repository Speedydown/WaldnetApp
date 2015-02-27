using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldnet.Data.DataModel
{
    public class NewsItem
    {
        public string Header { get; private set; }
        public string Content { get; private set; }
        public string DateTimePosted { get; private set; }

        public NewsItem()
        {

        }
    }
}
