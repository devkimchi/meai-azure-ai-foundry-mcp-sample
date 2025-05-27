using System.ClientModel;

using Azure.AI.OpenAI;

using Meai.ClientApp.Components;

using Microsoft.Extensions.AI;

using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionstring = config.GetConnectionString("openai") ?? throw new InvalidOperationException("Missing connection string: openai.");
var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                   ?? throw new InvalidOperationException("Missing endpoint.");
var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                 ?? throw new InvalidOperationException("Missing API key.");

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var credential = new ApiKeyCredential(apiKey);
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri(endpoint),
};

var openAIClient = endpoint.TrimEnd('/').Equals("https://models.inference.ai.azure.com")
                   ? new OpenAIClient(credential, openAIOptions)
                   : new AzureOpenAIClient(new Uri(endpoint), credential);
var chatClient = openAIClient.GetChatClient(config["OpenAI:DeploymentName"]!).AsIChatClient();

builder.Services.AddChatClient(chatClient)
                .UseLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
