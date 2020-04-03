using Gloaming.DiscordBot.DiscordBot.Models;
using System.Collections.Generic;

namespace Gloaming.DiscordBot.DiscordBot
{
    public class Constants
    {
        public static Dictionary<int, string> MajorArcana = new Dictionary<int, string>
        {
            { 0, "The Fool" },
            { 1, "The Magician" },
            { 2, "The High Priestess" },
            { 3, "The Empress" },
            { 4, "The Emperor" },
            { 5, "The Hierophant" },
            { 6, "The Lovers" },
            { 7, "The Chariot" },
            { 8, "Strength" },
            { 9, "The Hermit" },
            { 10, "Wheel of Fortune" },
            { 11, "Justice" },
            { 12, "The Hanged Man" },
            { 13, "Death" },
            { 14, "Temperance" },
            { 15, "The Devil" },
            { 16, "The Tower" },
            { 17, "The Star" },
            { 18, "The Moon" },
            { 19, "The Sun" },
            { 20, "Judgement" },
            { 21, "The World" },
        };

        public static List<Suit> Suits = new List<Suit>
        {
            { new Suit { Name = "Wands", Effect = "2 hits", Trumps = new List<string> { "Swords" } } },
            { new Suit { Name = "Swords", Effect = "Pressure", Trumps = new List<string> { "Cups" } } },
            { new Suit { Name = "Cups", Effect = "None", Trumps = new List<string> { "Wands", "Pentacles" } } },
            { new Suit { Name = "Pentacles", Effect = "No hit, Recover", Trumps = new List<string> { "Wands", "Swords" } } }
        };
    }
}
