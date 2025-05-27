using Azure.AI.OpenAI;

using Meai.ClientApp.Components;
using Meai.ClientApp.Extensions;

using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var chatClient = config["MEAI:ChatClient"]! switch
{
    "openai" => builder.GetAzureOpenAIChatClient(),
    "anthropic" => builder.GetAnthropicChatClient(),
    "google" => builder.GetGoogleChatClient(),
    _ => throw new InvalidOperationException("Unknown chat client.")
};

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
