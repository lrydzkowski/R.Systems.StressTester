namespace R.Systems.StressTester.Cli.Models;

internal class SendRequestsData
{
    public string Url { get; init; } = "";

    public string HttpMethod { get; init; } = "";

    public object? Body { get; init; }

    public List<SendRequestsHeader> Headers { get; init; } = new();

    public SendRequestsTimeInfo TimeInfo { get; init; } = new();
}

internal class SendRequestsHeader
{
    public string Name { get; init; } = "";

    public string Value { get; init; } = "";
}

internal class SendRequestsTimeInfo
{
    public int Requests { get; init; } = 0;

    public int IntervalInSeconds { get; init; } = 0;

    public int TimeInSeconds { get; init; } = 0;
}
