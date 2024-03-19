using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;

namespace Workbench.Orleans.Streaming;

[ImplicitStreamSubscription("StringStream")]
public class ImplicitStringGrain(ILogger<ImplicitGuidGrain> logger) : Grain, IGrainWithStringKey
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("StreamProvider");
        var streamId = StreamId.Create("StringStream", this.GetPrimaryKeyString());
        var stream = streamProvider.GetStream<string>(streamId);
        await stream.SubscribeAsync(
            (data, _) =>
            {
                logger.LogInformation("Received: {Data} on grain with key {Key}",
                    data, this.GetPrimaryKeyString());
                return Task.CompletedTask;
            });
    }
}