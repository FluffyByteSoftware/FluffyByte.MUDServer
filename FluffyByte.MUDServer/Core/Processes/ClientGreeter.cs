using FluffyByte.MUDServer.Core.Helpers;
using FluffyByte.MUDServer.Core.IO.Networking.Client;

namespace FluffyByte.MUDServer.Core.Processes;

public sealed class ClientGreeter : IFluffyCoreProcess
{
    public string Name => "Walmart Greeter";
    public bool GreetNewUsers { get; set; } = true;
    
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public FluffyCoreProcessState State { get; private set; } = FluffyCoreProcessState.Stopped;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (State == FluffyCoreProcessState.Running)
        {
            Scribe.Debug($"Process {Name} is already running. Cannot initialize.");
            return;
        }
        
        GreetNewUsers = true;
        
        await Task.CompletedTask;
    }

    public Task RequestStartAsync(CancellationToken cancellationToken = default)
    {
        if (State == FluffyCoreProcessState.Running)
            return Task.CompletedTask;

        State = FluffyCoreProcessState.Running;
        // Start logic here
        return Task.CompletedTask;
    }

    public Task RequestStopAsync(CancellationToken cancellationToken = default)
    {
        if (State == FluffyCoreProcessState.Stopped)
            return Task.CompletedTask;

        State = FluffyCoreProcessState.Stopped;
        // Stop logic here
        return Task.CompletedTask;
    }

    public async Task WelcomeNewCustomer(IFluffyClient client)
    {
        if (!GreetNewUsers)
        {
            await client.Messenger.SendMessageAsync("The MUD is currently not accepting users. " +
                                                    "Please try again later.");
            await client.RequestDisconnectAsync();
            return;
        }
        
        try
        {
            await client.Messenger.SendMessageAsync("Welcome to the FluffyByte MUD!");
            await client.Messenger.SendMessageAsync("Please enter your name:");
            
            var name = await client.Messenger.ReadMessageAsync();
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                await client.Messenger.SendMessageAsync("Invalid name. Disconnecting.");
                await client.RequestDisconnectAsync();
                return;
            }

            await client.Messenger.SendMessageAsync($"Hello, {name}! Enjoy your stay.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }
}