namespace TallyMCP.Configuration;

public class TallyMcpOptions
{
    public const string SectionName = "TallyMCP";

    public ServerOptions Server { get; set; } = new();
    public TallyOptions Tally { get; set; } = new();
}

public class ServerOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3001;

    public string GetUrl() => $"http://{Host}:{Port}";
}

public class TallyOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 9000;

    public string GetUrl() => $"http://{Host}:{Port}";
}