using System.ClientModel;

using Anthropic.SDK;

using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;

using Mscc.GenerativeAI.Microsoft;

using OpenAI;

namespace Meai.ClientApp.Extensions;

public static class IChatClientExtensions
{
    public static IChatClient GetAzureOpenAIChatClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var connectionstring = config.GetConnectionString("openai") ?? throw new InvalidOperationException("Missing connection string: openai.");
        var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing endpoint.");
        var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing API key.");

        var credential = new ApiKeyCredential(apiKey);
        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint),
        };

        var openAIClient = (endpoint.TrimEnd('/').Equals("https://models.inference.ai.azure.com") ||
                            endpoint.TrimEnd('/').Equals("https://models.github.ai/inference") ||
                            endpoint.TrimEnd('/').StartsWith("https://api.openai.com"))
                        ? new OpenAIClient(credential, openAIOptions)
                        : new AzureOpenAIClient(new Uri(endpoint), credential);
        var chatClient = openAIClient.GetChatClient(config["OpenAI:DeploymentName"]!).AsIChatClient();

        return chatClient;
    }

    public static IChatClient GetAnthropicChatClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var connectionstring = config.GetConnectionString("anthropic") ?? throw new InvalidOperationException("Missing connection string: anthropic.");
        var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing endpoint.");
        var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing API key.");

        var apiKeys = new APIAuthentication(apiKey);
        var anthropicClient = new AnthropicClient(apiKeys);
        var chatClient = anthropicClient.Messages
                                        .AsBuilder()
                                        .Build();
        return chatClient;
    }

    public static IChatClient GetGoogleChatClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var connectionstring = config.GetConnectionString("google") ?? throw new InvalidOperationException("Missing connection string: google.");
        var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing endpoint.");
        var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                        ?? throw new InvalidOperationException("Missing API key.");

        var chatClient = new GeminiChatClient(apiKey, config["Google:ModelName"]!);

        return chatClient;
    }
}
