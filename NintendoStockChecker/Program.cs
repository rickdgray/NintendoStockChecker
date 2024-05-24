using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NintendoStockChecker;

var builder = Host.CreateApplicationBuilder();
builder.Logging.AddConsole();
builder.Services.AddHostedService<NintendoStockChecker.NintendoStockChecker>();
builder.Services.AddOptions<Settings>()
    .Bind(builder.Configuration.GetSection(nameof(Settings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Build().Run();
