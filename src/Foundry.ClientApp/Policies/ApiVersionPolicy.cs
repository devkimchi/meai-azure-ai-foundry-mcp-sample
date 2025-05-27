using Azure.Core;
using Azure.Core.Pipeline;

namespace Foundry.ClientApp.Policies;

public class ApiVersionPolicy(string apiVersion) : HttpPipelinePolicy
{
    public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        message.Request.Uri.Query = $"?api-version={apiVersion}";
        ProcessNext(message, pipeline);
    }

    public override ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        message.Request.Uri.Query = $"?api-version={apiVersion}";
        var task = ProcessNextAsync(message, pipeline);

        return task;
    }
}
