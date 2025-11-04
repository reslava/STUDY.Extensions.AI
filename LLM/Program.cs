
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OllamaSharp;

var builder = Host.CreateApplicationBuilder ();

var client = builder.Services.AddChatClient (new OllamaApiClient (new Uri ("http://localhost:11434"), "llama3.1")); //llama3.1   phi3:mini
client.UseDistributedCache 
    (new MemoryDistributedCache(Options.Create (new MemoryDistributedCacheOptions ())));

var host = builder.Build ();

var chatClient = host.Services.GetRequiredService<IChatClient> ();

ChatOptions statefulOptions = new () { ConversationId = "my-conversation-id" };
while (true)
{
    Console.Write ("Q: ");
    ChatMessage message = new (ChatRole.User, Console.ReadLine ());
    Console.WriteLine (statefulOptions.ConversationId);
    Console.WriteLine (await chatClient.GetResponseAsync (message, statefulOptions));
}

//Console.WriteLine (await client.GetResponseAsync (
//[
//    new(ChatRole.System, "You are a helpful AI assistant"),
//    new(ChatRole.User, "What is AI?"),
//]));

//List<ChatMessage> history = [];
//while (true)
//{
//    Console.Write ("Q: ");
//    history.Add (new (ChatRole.User, Console.ReadLine ()));

//    ChatResponse response = await client.GetResponseAsync (history);
//    Console.WriteLine (response);

//    history.AddMessages (response);
//}