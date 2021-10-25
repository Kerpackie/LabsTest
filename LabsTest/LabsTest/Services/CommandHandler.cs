using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LabsTest.Services
{
    public class CommandHandler : DiscordClientService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CommandHandler> _logger;
            
        public CommandHandler(
            IServiceProvider provider,
            DiscordSocketClient client,
            CommandService service,
            IConfiguration configuration,
            ILogger<CommandHandler> logger) 
            : base(client, logger)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _configuration = configuration;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.MessageReceived += OnMessageRecieved;
            _service.CommandExecuted += OnCommandExecuted;
            
            await _service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                services: _provider);
        }

        private static async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
            {
                Console.WriteLine("Running Successfully.");
                return;
            }

            Console.WriteLine("There is an Error:");
            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
            Console.WriteLine("Does it reach here.");
        }

        private async Task OnMessageRecieved(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message)
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasStringPrefix(_configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }
    }
}