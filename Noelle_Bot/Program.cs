using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Noelle_Bot.Handlers;
using Noelle_Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace Noelle_Bot
{
    internal class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private LavaNode _lavaNode;
        private LavaLinkAudio _audioService;
        private GlobalData _globalData;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            //var config = new DiscordSocketConfig();
            //config.GatewayIntents = GatewayIntents.All;
            //config.AlwaysDownloadUsers = true;
            //_client = new DiscordSocketClient(config);
            //_commands = new CommandService();

            // Configure services
            _services = ConfigureServices();

            // Initialize services
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commands = _services.GetRequiredService<CommandService>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _globalData = _services.GetRequiredService<GlobalData>();
            _audioService = _services.GetRequiredService<LavaLinkAudio>();

            // Subscribe LavaLink events
            _lavaNode.OnLog += _client_Log;
            _lavaNode.OnTrackEnded += _audioService.TrackEnded;

            // Subscribe Discord events
            _client.Ready += _client_Ready;
            _client.Log += _client_Log;

            await _globalData.InitializeAsync();

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, GlobalData.Config.DiscordToken);

            await _client.StartAsync();

            await Task.Delay(-1);


        }

        private async Task _client_Ready()
        {
            try
            {
                await _lavaNode.ConnectAsync();
                await _client.SetGameAsync(GlobalData.Config.GameStatus);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<LavaNode>()
                .AddSingleton(new LavaConfig())
                .AddSingleton<LavaLinkAudio>()
                .AddSingleton<GlobalData>()
                .BuildServiceProvider();
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var Message = arg as SocketUserMessage;
            if (Message == null) return;
            var context = new SocketCommandContext(_client, Message);

            if (Message.Author.IsBot) return;


            int argPos = 0;
            if (Message.HasStringPrefix(".", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await Message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
