using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waldnet.Data.DataModel
{
    public class NewsLink
    {
        public string URL { get; private set; }
        public string Name { get; private set; }
        
        public NewsLink(string URL, string Name)
        {
            this.URL = "http://waldnet.nl" + URL;
            this.Name = Name; 
        }
    }
}
