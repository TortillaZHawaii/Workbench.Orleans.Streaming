using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

var builder = Host.CreateApplicationBuilder(args);

builder.UseOrleans(siloBuilder =>
{
   siloBuilder
       .UseLocalhostClustering()
       .AddMemoryGrainStorage("PubSubStore")
       .AddMemoryStreams("StreamProvider");
});

using var host = builder.Build();

await host.StartAsync();

var clusterClient = host.Services.GetRequiredService<IClusterClient>();
var streamProvider = clusterClient.GetStreamProvider("StreamProvider");

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Sending messages to streams");

var guidNamespace = "GuidStream";
var guidStream = streamProvider.GetStream<string>(guidNamespace, new Guid());
await guidStream.OnNextAsync("Hello, World!");

var stringNamespace = "StringStream";
var stringStream = streamProvider.GetStream<string>(stringNamespace, "Hello grain Id");
await stringStream.OnNextAsync("Hello, World!");

logger.LogInformation("Sent messages to streams");

await Task.Delay(4000);

await host.StopAsync();
