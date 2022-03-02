using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Noelle_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong! UwU");
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "You dwont hwave twe pwermission! qwq")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("Pweasy spwecify a wuser! OwO"); 
                return;
            }
            if (reason == null) reason = "Not specified, you just suck! UwU";

            await Context.Guild.AddBanAsync(user, 0, reason);

            var EmbedBuilder = new EmbedBuilder()
                .WithDescription($":white_check_mark: {user.Mention} was bwanned UwU\n **Reason** {reason}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }
    }
}
