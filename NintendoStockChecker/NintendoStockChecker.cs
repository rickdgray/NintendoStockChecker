using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NintendoStockChecker.Models;
using System.Net.Http.Json;

namespace NintendoStockChecker
{
    internal class NintendoStockChecker(IOptions<Settings> settings,
        ILogger<NintendoStockChecker> logger) : BackgroundService
    {

        private readonly Settings _settings = settings.Value;
        private readonly ILogger<NintendoStockChecker> _logger = logger;

        // as per new httpclient guidelines and updated digest auth for .NET 6+
        // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
        private static readonly HttpClient _client = new(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        });

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting application.");
            await PushNotification("Starting application.", cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Polling Nintendo Stock.");

                NintendoResponse? productStatus = null;

                try
                {
                    productStatus = await _client.GetFromJsonAsync<NintendoResponse>("https://graph.nintendo.com/?operationName=ProductsBySku&variables={\"locale\":\"en_US\",\"personalized\":false,\"skus\":[\"117806\"]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"38a6ad9c4e61fc840abcaf65021262b6122c52040051acb97f07846d2cd7099c\"}}", cancellationToken);
                }
                catch
                {
                }

                if (productStatus != null && productStatus.Data.Products.Count != 0)
                {
                    var product = productStatus.Data.Products.First();

                    _logger.LogDebug("Found product: {product.Name}", product.Name);

                    if (product.IsSalableQty)
                    {
                        await PushNotification($"{product.Name} is now available!", cancellationToken);
                    }
                }

                await Task.Delay(_settings.PollRateInSeconds * 1000, cancellationToken);
            }
        }

        private async Task PushNotification(string message, CancellationToken cancellationToken = default)
        {
            // TODO: customizable notification text
            var notification = new Dictionary<string, string>
            {
                { "token", _settings.PushoverAppKey },
                { "user", _settings.PushoverUserKey },
                { "title", "Nintendo Stock Checker" },
                { "message", message }
            };

            try
            {
                await _client.PostAsync(
                    "https://api.pushover.net/1/messages.json",
                    new FormUrlEncodedContent(notification),
                    cancellationToken);
            }
            catch (HttpRequestException)
            {
                //TODO: retry logic
            }
            catch (TaskCanceledException ex)
            {
                //TODO: logging these for the time being for debugging
                _logger.LogInformation(ex, "Task Cancelled Exception");
            }

            _logger.LogInformation("Notification pushed.");
        }
    }
}
