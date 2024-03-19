using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;

namespace Workbench.Orleans.Streaming;

[ImplicitStreamSubscription("GuidStream")]
public class ImplicitGuidGrain(ILogger<ImplicitGuidGrain> logger) : Grain, IGrainWithGuidKey
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("StreamProvider");
        var streamId = StreamId.Create("GuidStream", this.GetPrimaryKey());
        var stream = streamProvider.GetStream<string>(streamId);
        await stream.SubscribeAsync(
            (data, _) =>
            {
                logger.LogInformation("Received: {Data} on grain with key {Key}",
                    data, this.GetPrimaryKey());
                return Task.CompletedTask;
            });
    }
}