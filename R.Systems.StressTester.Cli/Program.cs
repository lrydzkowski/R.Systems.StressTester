using Cocona;
using Cocona.Builder;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.StressTester.Cli.Commands;

CoconaAppBuilder builder = CoconaApp.CreateBuilder();
builder.Services.AddTransient<SendRequestsCommands>();
builder.Services.AddHttpClient();

CoconaApp app = builder.Build();
app.AddCommands<SendRequestsCommands>();
app.Run();
