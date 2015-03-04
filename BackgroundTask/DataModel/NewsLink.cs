using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask
{
    public sealed class NewsLink
    {
        public string URL { get; private set; }
        public string Name { get; private set; }
        public string Image { get; private set;}

        public NewsLink(string URL, string Name, string Image)
        {
            if (Image == string.Empty)
            {
                Image = "Assets/NewsLogo.png";
            }

            this.URL = "http://waldnet.nl" + URL;
            this.Name = Name;
            this.Image = Image;
        }

        public void SetImage(string Image)
        {
            this.Image = Image;
        }
    }
}
