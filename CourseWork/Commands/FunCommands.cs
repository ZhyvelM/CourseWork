using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        public async Task PingPong(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("response")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Content.Contains("daun")).ConfigureAwait(false);

            if (message.Result != null)
                await ctx.Channel.SendMessageAsync("true").ConfigureAwait(false);
        }

        [Command("ball")]
        [Description("only true answers on your questions")]
        public async Task Ball(CommandContext ctx, [RemainingText] string str)
        {
            Random rnd = new Random();
            int value = rnd.Next(0, 5);
            if (value == 0) { await ctx.Channel.SendMessageAsync("Yes").ConfigureAwait(false); }
            else if (value == 1) { await ctx.Channel.SendMessageAsync("Maybe").ConfigureAwait(false); }
            else if (value == 2) { await ctx.Channel.SendMessageAsync("Very doubtful").ConfigureAwait(false); }
            else if (value == 3) { await ctx.Channel.SendMessageAsync("It is decidedly so").ConfigureAwait(false); }
            else if (value == 4) { await ctx.Channel.SendMessageAsync("Most likely").ConfigureAwait(false); }
            else { await ctx.Channel.SendMessageAsync("No").ConfigureAwait(false); }
        }

        [Command("flip")]
        [Description("coin flip")]
        public async Task CoinFlip(CommandContext ctx)
        {
            Random rnd = new Random();
            if (rnd.Next(1, 100) % 2 == 0)
            {
                await ctx.Channel.SendMessageAsync("Heads:eagle:").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Tails:chestnut:").ConfigureAwait(false);
            }
        }
    }
}
