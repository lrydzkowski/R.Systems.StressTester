namespace R.Systems.StressTester.Cli.Models;

internal class ResponseInfo
{
    public string? HttpMethod { get; init; }

    public string? Url { get; init; }

    public int? StatusCode { get; init; }

    public string? ErrorMsg { get; init; }

    public bool Error { get; init; } = false;
}
