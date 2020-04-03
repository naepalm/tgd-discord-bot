using System;
using System.Collections.Generic;
using System.Text;

namespace Gloaming.DiscordBot.DiscordBot.Models
{
    public class CombatThrow
    {
        public string ThrownBy { get; set; }

        public string Suit { get; set; }

        public string Target { get; set; }

        public string Maneuver { get; set; }
    }
}
