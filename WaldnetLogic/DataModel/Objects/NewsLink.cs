﻿using BaseLogic.Xaml_Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaldnetLogic
{
    public sealed class NewsLink : INewsLink
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

            if (URL.StartsWith("/"))
            {
                this.URL = "http://waldnet.nl" + URL;
            }
            else
            {
                this.URL = "http://waldnet.nl/wn/nieuws/" + URL;
            }

            this.Name = Name;
            this.Image = Image;
        }

        public void SetImage(string Image)
        {
            this.Image = Image;
        }


        public string ImageURL
        {
            get { return Image; }
        }

        public string Title
        {
            get { return Name; }
        }

        public string Content
        {
            get { return string.Empty; }
        }

        public string CommentCount
        {
            get { return string.Empty; }
        }

        public string Time
        {
            get { return string.Empty; }
        }
    }
}