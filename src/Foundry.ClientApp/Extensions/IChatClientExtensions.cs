using System.ClientModel;

using Azure;
using Azure.AI.Inference;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;

using OpenAI;

namespace Foundry.ClientApp.Extensions;

public static class IChatClientExtensions
{
    public static IChatClient GetAIFoundryChatClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var connectionstring = config.GetConnectionString("foundry") ?? throw new InvalidOperationException("Missing connection string: foundry.");
        var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing endpoint.");
        var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing API key.");

        var credential = new AzureKeyCredential(apiKey);
        var options = new AzureAIInferenceClientOptions()
                          .AddApiVersionPolicy(config["AzureAIFoundry:ApiVersion"]!);
        var foundryClient = new ChatCompletionsClient(new Uri(endpoint), credential, options);
        var chatClient = foundryClient.AsIChatClient(config["AzureAIFoundry:DeploymentName"]!);

        return chatClient;
    }

    public static async Task<IChatClient> GetFoundryLocalChatClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var alias = config["FoundryLocal:ModelAlias"]!;
        var manager = await FoundryManager.StartModelAsync(alias);
        var model = await manager.GetModelInfoAsync(alias);

        var credential = new ApiKeyCredential(manager.ApiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = manager.Endpoint
        };
        var foundryClient = new OpenAIClient(credential, options);
        var chatClient = foundryClient.GetChatClient(model?.ModelId).AsIChatClient();

        return chatClient;
    }
}
