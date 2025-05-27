using Azure;
using Azure.AI.Inference;

using Foundry.ClientApp.Components;
using Foundry.ClientApp.Extensions;

using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionstring = config.GetConnectionString("foundry") ?? throw new InvalidOperationException("Missing connection string: foundry.");
var endpoint = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                   ?? throw new InvalidOperationException("Missing endpoint.");
var apiKey = connectionstring.Split(';').FirstOrDefault(x => x.StartsWith("Key=", StringComparison.InvariantCultureIgnoreCase))?.Split('=')[1]
                 ?? throw new InvalidOperationException("Missing API key.");

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var credential = new AzureKeyCredential(apiKey);
var options = new AzureAIInferenceClientOptions()
                  .AddApiVersionPolicy(config["AzureAIFoundry:ApiVersion"]!);
var foundryClient = new ChatCompletionsClient(new Uri(endpoint), credential, options);
var chatClient = foundryClient.AsIChatClient(config["AzureAIFoundry:DeploymentName"]!);

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
