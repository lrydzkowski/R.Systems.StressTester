using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using R.Systems.StressTester.Cli.Models;
using R.Systems.StressTester.Cli.Validation;

namespace R.Systems.StressTester.Cli.Commands;

internal class SendRequestsCommands
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendRequestsCommands> _logger;
    private readonly IList<string> _statusCodes = new List<string>();

    public SendRequestsCommands(IHttpClientFactory httpClientFactory, ILogger<SendRequestsCommands> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendAsync([FilePathExists] string filePath)
    {
        SendRequestsData? data = await GetDataAsync(filePath);
        if (data == null)
        {
            _logger.LogWarning("Send requests data is empty");

            return;
        }

        await SendRequestsAsync(data);
    }

    private async Task<SendRequestsData?> GetDataAsync(string filePath)
    {
        string fileContent = await File.ReadAllTextAsync(filePath);
        SendRequestsData? fileData = JsonSerializer.Deserialize<SendRequestsData>(
            fileContent,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        return fileData;
    }

    private async Task SendRequestsAsync(SendRequestsData data)
    {
        List<Task> tasks = new();
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(data.TimeInfo.TimeInSeconds))
        {
            for (int i = 0; i < data.TimeInfo.Requests; i++)
            {
                tasks.Add(SendRequestAsync(data));
            }

            await Task.Delay(TimeSpan.FromSeconds(data.TimeInfo.IntervalInSeconds));
        }

        await Task.WhenAll(tasks);
    }

    private async Task SendRequestAsync(SendRequestsData data)
    {
        HttpRequestMessage message = new(new HttpMethod(data.HttpMethod), data.Url);
        foreach (SendRequestsHeader header in data.Headers)
        {
            message.Headers.Add(header.Name, header.Value);
        }

        if (data.Body != null)
        {
            message.Content = new StringContent(data.Body);
        }

        HttpClient httpClient = _httpClientFactory.CreateClient();
        HttpResponseMessage response = await httpClient.SendAsync(message);
        _statusCodes.Add(response.StatusCode.ToString());
    }
}
