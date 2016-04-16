using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaldnetLogic
{
    public sealed class Reaction
    {
        public string Sender { get; private set; }
        public string TimePosted { get; private set; }
        public string Content { get; private set; }

        public Reaction(string Sender, string TimePosted, string Content)
        {
            this.Sender = Sender;
            this.TimePosted = TimePosted;
            this.Content = Content;
        }
    }
}
