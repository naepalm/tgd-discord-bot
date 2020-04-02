using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Gloaming.DiscordBot.DiscordBot.Modules
{
    [Name("Card Draws")]
    public class CardDrawsModule : ModuleBase<SocketCommandContext>
    {
        [Command("card")]
        [Summary("Draws a card from the major arcana in the standard Tarot deck. Optional: Add your `trait (a number)` and `b` for blessed or `c` for cursed (ie: tgd!card 4 b).")]
        public async Task DrawCard(int? trait = null, string blessedOrCursed = null)
        {
            var random = new Random();
            var draw = random.Next(0, 21);
            bool isBlessed = blessedOrCursed == "b";
            bool isCursed = blessedOrCursed == "c";
            var total = draw;
            var result = $"{Context.Guild.GetUser(Context.User.Id).Nickname} drew **{Constants.MajorArcana[draw]} ({draw})**";

            if (trait.HasValue)
            {
                total = draw + trait.Value;
                if (isBlessed)
                {
                    total = total + 3;
                }
                if (isCursed)
                {
                    total = total - 3;
                }
            }

            switch (draw)
            {
                case 0:
                    if (trait.HasValue)
                    {
                        total = draw + trait.Value;
                        if (isBlessed)
                        {
                            total = total - 3;
                        }
                        if (isCursed)
                        {
                            total = total + 3;
                        }
                        result += $" for a total of **{total}**";
                    }
                    break;
                case 10:
                    result += ". That's a trump; automatic success! :sparkles: :raised_hands:";
                    break;
                case 16:
                    result += ". That's a dramatic failure :person_facepalming:";
                    break;
                default:
                    if (trait.HasValue)
                    {
                        result += $" for a total of **{total}**.";
                    }
                    break;
            }
            
            await ReplyAsync(result);
        }
    }
}
