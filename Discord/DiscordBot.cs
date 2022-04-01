using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace DefiKindom_QuestRunner.Discord
{
    internal class DiscordBot
    {
        private ulong channelId = 959496992499847169;
        private DiscordSocketClient _client;

        public DiscordBot()
        {
            _client = new DiscordSocketClient();
        }

        public async Task Start()
        {
            await _client.LoginAsync(TokenType.Bot, "OTU5NDk1NjQ0MjAzMDczNTU2.Ykct9A.9D_WyudUFC2s3t6uoqLJF0CwP5c");
            await _client.StartAsync();

            //_client.SlashCommandExecuted += ClientOnSlashCommandExecuted;
        }

        public async Task SendMessage(string msg)
        {
            var guildChannel = _client.Guilds.FirstOrDefault();
            if (guildChannel != null)
                await guildChannel.DefaultChannel.SendMessageAsync(msg);
        }
    }
}
