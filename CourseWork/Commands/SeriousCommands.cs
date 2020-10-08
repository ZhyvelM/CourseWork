using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CourseWork.Logic;
using System.Runtime.InteropServices.ComTypes;

namespace CourseWork.Commands
{
    public class SeriousCommands : BaseCommandModule
    {
        Logic.Logic lg = Logic.Logic.GetInstanse();        

        [Command("join")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        public async Task Join(CommandContext ctx)
        {
            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");
            await chn.ConnectAsync();

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:"));
        }

        [Command("leave")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            vnc.Disconnect();
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:"));
        }

        [Command("start")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        [Description("This is for starting checking attendance")]
        public async Task Start(CommandContext ctx, TimeSpan duration)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var chn = ctx.Member?.VoiceState?.Channel;
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                if (chn == null)
                    throw new InvalidOperationException("You need to be in a voice channel.");
                await chn.ConnectAsync();
            }

            var participants = chn.Users.ToList();
            for (int i =0; i < participants.Count; i++)
            {
                if (participants[i].IsBot == true) participants.Remove(participants[i]);
            }
            lg.setParticipants(participants, ctx.Guild, duration);
        }

        [Command("end")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        [Description("This is to end checking attendance")]
        public async Task End(CommandContext ctx)
        {
            var participants = lg.getParticipants(ctx.Guild);
            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
            foreach (var u in participants)
            {
                await userChannel.SendMessageAsync($"{u.DisplayName} was at the beginnig").ConfigureAwait(false);
            }
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            vnc.Disconnect();
        }

        [Command("quickcheck")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        [Description("This is for quick check")]
        public async Task QuickCheck(CommandContext ctx, TimeSpan duration)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Quick check",
                Description = "Mark your attendance before time is over"
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")).ConfigureAwait(false);

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);

            Reaction emoji = result.First();
            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            foreach (var r in result)
            {
                foreach (var u in r.Users)
                {
                    if (!u.IsBot)
                        if (r.Emoji == DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"))
                        {
                            var username = ctx.Guild.GetMemberAsync(u.Id).Result.DisplayName;
                            await userChannel.SendMessageAsync($"{username} is here\n").ConfigureAwait(false);
                        }
                }
            }
        }

        [Command("test")]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Преподаватель")]
        [Description("This is for knowledge test")]
        public async Task Test(CommandContext ctx, [RemainingText] string str)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var chn = ctx.Member?.VoiceState?.Channel;
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                if (chn == null)
                    throw new InvalidOperationException("You need to be in a voice channel.");
                await chn.ConnectAsync();
            }
            var participants = chn.Users.ToList();
            foreach (var u in participants)
            {
                if(!u.IsBot)
                test(ctx, u, str);
            }
        }

        private async Task test(CommandContext ctx, DiscordMember u, string str)
        {
            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Quick test",
                Description = str
            };
            var interactivity = ctx.Client.GetInteractivity();
            var channel = await u.CreateDmChannelAsync().ConfigureAwait(false);
            var message = await channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));
            var result = await interactivity.CollectReactionsAsync(message, TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
            foreach (var r in result)
            {
                bool flag = false;
                foreach (var us in r.Users)
                {
                    if (!us.IsBot)
                        if (r.Emoji == DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")|| r.Emoji == DiscordEmoji.FromName(ctx.Client, ":x:"))
                        {
                            var username = ctx.Guild.GetMemberAsync(us.Id).Result.DisplayName;
                            await userChannel.SendMessageAsync($"{username} unswered {r.Emoji}\n").ConfigureAwait(false);
                            flag = true;
                            break;
                        }
                }
                if (flag) break;
            }
        }
        public static Task MemberJoin(VoiceStateUpdateEventArgs e)
        {
            var chn = e.Channel;
            Logic.Logic lg = Logic.Logic.GetInstanse();
            if (!lg.isStarted(e.Guild))
            {
                return Task.CompletedTask;
            }
            if (chn != null)
            {
                var _participants = chn.Users.ToList();
                for (int i = 0; i < _participants.Count; i++)
                {
                    if (_participants[i].IsBot == true) _participants.Remove(_participants[i]);
                }

                lg.updateParticipants(_participants, e.Guild);
                
            }
            return Task.CompletedTask;
        }
    }
}
