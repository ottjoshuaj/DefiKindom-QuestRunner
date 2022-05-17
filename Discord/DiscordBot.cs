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
            await _client.LoginAsync(TokenType.Bot, "");
            await _client.StartAsync();

            _client.SlashCommandExecuted += ClientOnSlashCommandExecuted;
        }

        private Task ClientOnSlashCommandExecuted(SocketSlashCommand arg)
        {
            switch (arg.CommandName.ToLower())
            {
                case "restart": //Engine seems stuck so lets restart it
                    //TODO:  Go and 
                    break;

                case "reset": //Reet a given instance (unstick a timer etc)
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task SendMessage(string msg)
        {
            try
            {
                //var guildChannel = _client.Guilds.FirstOrDefault();
                //if (guildChannel != null)
                //    await guildChannel.DefaultChannel.SendMessageAsync(msg);
            }
            catch
            {

            }
        }
    }
}
