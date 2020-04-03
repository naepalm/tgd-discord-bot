using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Discord;
using System.Linq;
using Gloaming.DiscordBot.DiscordBot.Models;
using System.Collections.Generic;

namespace Gloaming.DiscordBot.DiscordBot.Modules
{
    public class CombatModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;
        private readonly DiscordSocketClient _discord;

        public CombatModule(DiscordSocketClient discord, CommandService service, IConfigurationRoot config)
        {
            _discord = discord;
            _service = service;
            _config = config;
        }

        [Command("listsuits")]
        [Summary("Lists all the suits, their effects, and what they trump")]
        public async Task ListSuitsAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are all the combat suits, their effects, and what they trump."
            };

            foreach (var suit in Constants.Suits)
            {
                builder.AddField(x =>
                {
                    x.Name = $"{suit.Name}";
                    x.Value = $"Trumps: {string.Join(", ", suit.Trumps)}\n Effect: {suit.Effect}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("startcombat")]
        [Summary("Starts a round of combat. Optional: add the round - `tgd!startcombat 1`")]
        public async Task StartCombatAsync(int? round = null)
        {
            var description = $"Combat has started!";
            if (round.HasValue)
            {
                description = $"Combat has started! **(Round {round.Value})**";
            }
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = description
            };

            builder.AddField(x =>
            {
                x.Name = $"Please throw in the following format:";
                x.Value = "`+||\"Character\" \"Suit\" \"Target\" \"Technique\"||`";
                x.IsInline = false;
            });

            builder.AddField(x =>
            {
                x.Name = "Requirements";
                x.Value = "* Quotations are required around all fields or the bot cannot parse them.\n" +
                "* Character: Used to parse results - keep it consistent for both your name and others targetting you\n" +
                "* Suit: Wands, Swords, Cups, or Pentacles. To see what they do, use `tgd!listsuits`\n" +
                "* Target: Your opponent, and should match a decided list of character names \n" +
                "* Technique: Optional, leave it blank or put in \"None\" if you're not using a Technique";
                x.IsInline = false;
            });

            builder.AddField(x =>
            {
                x.Name = "Throwing for NPCs";
                x.Value = "Talespinners can throw for all NPCs in one post if they separate it by lines with a spoiler tag around all of it, like so:\r " +
                "`+||\"Hound 1\" \"Swords\" \"Margo\"\n" +
                "\"Hound 2\" \"Wands\" \"Addie\" \"Gore\"||`";
            });

            await ReplyAsync("", false, builder.Build());
        }

        [Command("endcombat")]
        [Summary("Ends a round of combat and reveals the results. Must be used after `tgd!startcombat`.")]
        public async Task EndCombatAsync()
        {
            var messages = Context.Channel.GetCachedMessages();

            var sortedMessages = messages.OrderByDescending(x => x.Timestamp).ToList();
            var startCombatMessage = sortedMessages.FirstOrDefault(x => x.Content.Contains("tgd!startcombat"));
            var startCombatIndex = sortedMessages.IndexOf(startCombatMessage);

            var combatDrawMessages = sortedMessages.Take(startCombatIndex + 1).Where(x => x.Content.StartsWith("+||"));
            var combatThrows = new List<CombatThrow>();

            foreach(var drawMessage in combatDrawMessages)
            {
                var draw = drawMessage.Content.Replace("+||", string.Empty).Replace("||", string.Empty);
                if (!draw.Contains("\n"))
                {
                    var separatedDraw = draw.Split("\" \"").Select(x => x.Replace(" \"", string.Empty).Replace("\"", string.Empty)).ToArray();
                    var character = separatedDraw[0];
                    var suit = separatedDraw[1];
                    var target = separatedDraw[2];
                    var technique = separatedDraw.Count() > 3 ? separatedDraw[3] : string.Empty;

                    combatThrows.Add(new CombatThrow
                    {
                        ThrownBy = character,
                        Suit = suit,
                        Maneuver = technique,
                        Target = target
                    });
                }
                else
                {
                    var npcThrows = draw.Split("\n");

                    foreach(var npcDraw in npcThrows)
                    {
                        var separatedDraw = npcDraw.Split("\" \"").Select(x => x.Replace(" \"", string.Empty).Replace("\"", string.Empty)).ToArray();
                        var character = separatedDraw[0];
                        var suit = separatedDraw[1];
                        var target = separatedDraw[2];
                        var technique = separatedDraw.Count() > 3 ? separatedDraw[3] : string.Empty;
                        combatThrows.Add(new CombatThrow
                        {
                            ThrownBy = character,
                            Suit = suit,
                            Maneuver = technique,
                            Target = target
                        });
                    }
                }
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Combat has ended! Here are the results."
            };


            var drawList = new List<string>();
            foreach (var draw in combatThrows)
            {
                var drawManeuver = !string.IsNullOrEmpty(draw.Maneuver) && draw.Maneuver.ToLower() != "none" ? $" ({draw.Maneuver})" : string.Empty;
                var target = combatThrows.FirstOrDefault(x => x.ThrownBy == draw.Target);

                var drawSuitDetails = Constants.Suits.FirstOrDefault(x => x.Name.ToLower() == draw.Suit.ToLower());
                var targetSuitDetails = Constants.Suits.FirstOrDefault(x => x.Name.ToLower() == target.Suit.ToLower());
                var resolution = string.Empty;

                if (drawSuitDetails.Trumps.Any(x => x.Equals(targetSuitDetails.Name)))
                {
                    resolution = $" {draw.ThrownBy} Trumps.";
                }
                else if (targetSuitDetails.Trumps.Any(x => x.Equals(drawSuitDetails.Name)))
                {
                    resolution = $" {draw.ThrownBy} Fails.";
                }
                else
                {
                    resolution = $" Characters bounce; nothing happens.";
                }

                if (!string.IsNullOrEmpty(drawManeuver))
                {
                    resolution += " (Check the Technique's text to make sure this is accurate).";
                }

                drawList.Add($"{draw.ThrownBy}: **{draw.Suit}{drawManeuver}** against {draw.Target}.{resolution}");
            }

            var combatThrowsDescription = $"Combat Throws";
            if (startCombatMessage.Content.Contains(" "))
            {
                var round = startCombatMessage.Content.Split(" ")[1];
                combatThrowsDescription = $"Combat Throws **(Round {round})**";
            }

            builder.AddField(x =>
            {
                x.Name = combatThrowsDescription;
                x.Value = string.Join("\n", drawList);
                x.IsInline = false;
            });

            await Context.Channel.DeleteMessageAsync(startCombatMessage.Id);
            await Context.Channel.DeleteMessageAsync(messages.FirstOrDefault(message => message.Embeds.Any(embed => embed.Description.Contains("Combat has started!"))));

            foreach(var message in combatDrawMessages)
            {
                await Context.Channel.DeleteMessageAsync(message.Id);
            }

            await Context.Channel.DeleteMessageAsync(messages.FirstOrDefault(x => x.Content.Equals("tgd!endcombat")));

            await ReplyAsync("", false, builder.Build());
        }
    }
}