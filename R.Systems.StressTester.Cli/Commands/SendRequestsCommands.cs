using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using R.Systems.StressTester.Cli.Models;
using R.Systems.StressTester.Cli.Validation;

namespace R.Systems.StressTester.Cli.Commands;

internal class SendRequestsCommands
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendRequestsCommands> _logger;
    private readonly List<ResponseInfo> _responses = new();

    public SendRequestsCommands(IHttpClientFactory httpClientFactory, ILogger<SendRequestsCommands> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendAsync([FilePathExists] string filePath, [DirectoryPathExists] string outDirectoryPath)
    {
        SendRequestsData? data = await GetDataAsync(filePath);
        if (data == null)
        {
            _logger.LogWarning("Send requests data is empty");

            return;
        }

        await SendRequestsAsync(data);
        await SaveResponsesAsync(outDirectoryPath);
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
        try
        {
            HttpRequestMessage message = new(new HttpMethod(data.HttpMethod), data.Url);
            foreach (SendRequestsHeader header in data.Headers)
            {
                message.Headers.Add(header.Name, header.Value);
            }

            if (data.Body != null)
            {
                message.Content = new StringContent(
                    JsonSerializer.Serialize(data.Body),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json
                );
            }

            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpClient.SendAsync(message);
            _responses.Add(
                new ResponseInfo
                {
                    HttpMethod = response.RequestMessage?.Method?.ToString(),
                    Url = response.RequestMessage?.RequestUri?.ToString(),
                    StatusCode = (int?)response.StatusCode
                }
            );
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
            {
                message += " " + ex.InnerException.Message;
            }

            _responses.Add(new ResponseInfo { ErrorMsg = message, Error = true });
        }
    }

    private async Task SaveResponsesAsync(string outDirectoryPath)
    {
        string serializedResponses = JsonSerializer.Serialize(_responses);
        await File.WriteAllTextAsync($"{outDirectoryPath}\\results.json", serializedResponses);
    }
}
