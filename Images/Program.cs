using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OllamaSharp;

var builder = Host.CreateApplicationBuilder ();

var client = builder.Services.AddChatClient (new OllamaApiClient (new Uri ("http://localhost:11434"), "gemma3:4b")); 
client.UseDistributedCache
    (new MemoryDistributedCache (Options.Create (new MemoryDistributedCacheOptions ())));

var host = builder.Build ();

var chatClient = host.Services.GetRequiredService<IChatClient> ();

foreach (var filePath in Directory.GetFiles ("images", "*.png"))
{
    var name = Path.GetFileNameWithoutExtension (filePath);
    var message = new ChatMessage (ChatRole.User,
        $"""
        Analyze the image and identify all objects present in it.
        Provide the status of traffic, number of cars, trucks, motorbikes, Cyclists and pedestrians.        
        """);
    message.Contents.Add (new DataContent (File.ReadAllBytes (filePath), "image/png"));
    var imagesResponse = await chatClient.GetResponseAsync<ImagesResult> (message);
    Console.WriteLine (imagesResponse.Result with { CameraName = name });
}


public record ImagesResult
{
    public string CameraName { get; init; }
    public string Objects { get; init; }
    public TrafficStatus Status { get; init; }
    public int Cars { get; init; }
    public int Trucks { get; init; }
    public int Motorbikes { get; init; }
    public int Cyclists { get; init; }
    public int Pedrestians { get; init; }

    public enum TrafficStatus
    {
        Empty,
        Low,
        Medium,
        High
    }
}
