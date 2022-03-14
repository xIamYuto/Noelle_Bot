using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Noelle_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        [Command("donate")]
        public async Task Donate()
        {
            await ReplyAsync("Twis is vewwy nwuice bot! Pwease dwonate if ywou want two bwuy me coffee uwu :coffee: \nhttps://www.paypal.me/esavcenko");
        }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong! UwU");
            await ReplyAsync($"Current Ping  {Context.Client.Latency}ms");
        }
        [Command("source")]
        [Alias("sourcecode", "src")]
        [Summary("I am mwade by Yuto OwO.")]
        public async Task Source()
    => await ReplyAsync($":heart: **{Context.Client.CurrentUser}** is based on this source code:\nhttps://github.com/xIamYuto/Noelle_Bot");

        [Command("kick")]
        [Summary("Kick a user from the server.")]
        [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "I dwont hwave twe pwermission! qwq")]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "Ywu dwont hwave twe pwermission! qwq")]
        public async Task Kick(SocketGuildUser targetUser, [Remainder] string reason = "No reason provided.")
        {
            await targetUser.KickAsync(reason);
            await ReplyAsync($"**{targetUser}** hwas kicked OwO :wave:");
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I dwont hwave twe pwermission! qwq")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "Ywu dwont hwave twe pwermission! qwq")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            Console.WriteLine(user.ToString());
            if (user == null)
            {
                await ReplyAsync("Pweasy spwecify a wuser! OwO");
                return;
            }
            if (reason == null) reason = "Not specified, you just suck! UwU";

            // await Context.Guild.AddBanAsync(user, 0, reason);
            
            await user.BanAsync(0, reason);


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