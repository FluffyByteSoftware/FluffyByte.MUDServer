using FluffyByte.MUDServer.Core.Helpers;

namespace FluffyByte.MUDServer.Core.IO.Networking.Client.Components;

public sealed class Messenger(IFluffyClient client) : IFluffyClientComponent
{
    private readonly StreamWriter _writer = 
        new StreamWriter(client.TcpClient.GetStream()) { AutoFlush = true };
    
    private readonly StreamReader _reader = 
        new StreamReader(client.TcpClient.GetStream());
    
    public string Name => "Messenger";
    public CancellationTokenSource Cts { get; set; } = new CancellationTokenSource();
    
    public async Task SendMessageAsync(string message, bool newline = true)
    {
        try
        {
            if (newline)
            {
                await _writer.WriteLineAsync(message);
            }
            else
            {
                await _writer.WriteAsync(message);
            }
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }

    public async Task<string> ReadMessageAsync()
    {
        try
        {
            var response = await _reader.ReadLineAsync();
            
            return response ?? string.Empty;
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
            return string.Empty;
        }
    }
}