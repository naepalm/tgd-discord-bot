using System;
using System.Collections.Generic;
using System.Text;

namespace Gloaming.DiscordBot.DiscordBot.Models
{
    public class Suit
    {
        public string Name { get; set; }

        public string Effect { get; set; }

        public List<string> Trumps { get; set; }
    }
}
