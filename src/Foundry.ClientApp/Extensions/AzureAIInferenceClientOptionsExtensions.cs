using Azure.AI.Inference;
using Azure.Core;
using Azure.Core.Pipeline;

using Foundry.ClientApp.Policies;

namespace Foundry.ClientApp.Extensions;

public static class AzureAIInferenceClientOptionsExtensions
{
    public static AzureAIInferenceClientOptions AddApiVersionPolicy(this AzureAIInferenceClientOptions options, string apiVersion)
    {
        options.AddPolicy(new ApiVersionPolicy(apiVersion), HttpPipelinePosition.PerCall);

        return options;
    }

    public static AzureAIInferenceClientOptions AddApiVersionPolicy(this AzureAIInferenceClientOptions options, HttpPipelinePolicy policy, HttpPipelinePosition position)
    {
        options.AddPolicy(policy, position);

        return options;
    }
}
