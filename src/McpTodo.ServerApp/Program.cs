using McpTodoList.ContainerApp.Data;
using McpTodoList.ContainerApp.Middlewares;
using McpTodoList.ContainerApp.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

var connection = new SqliteConnection("Filename=:memory:");
connection.Open();

builder.Services.AddSingleton(connection);

builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite(connection));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithToolsFromAssembly();

var app = builder.Build();

// Initialise the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable API key auth for MCP server.
app.UseMcpAuth();

// Configure the MCP endpoints.
app.MapMcp();

app.MapDefaultEndpoints();

app.Run();
