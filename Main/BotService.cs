using System;
using System.Threading;
using System.Threading.Tasks;
using DiscordUwuBot.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordUwuBot.Main;

/// <summary>
/// Runs the discord bot within a hosted service.
/// </summary>
/// <remarks>
/// This class can safely be instantiated multiple times. Each instance will create and use a unique DI scope. 
/// </remarks>
public sealed class BotService : IHostedService, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly BotMain _botMain;
    
    public BotService(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
        _botMain = _serviceScope.ServiceProvider.GetRequiredService<BotMain>();
    }

    public Task StartAsync(CancellationToken _) => _botMain.StartAsync();
    public Task StopAsync(CancellationToken _) => _botMain.StopAsync();
    
    public void Dispose() => _serviceScope.Dispose();
}