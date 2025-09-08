using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TallyMCP.Configuration;

public class TallyHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly TallyMcpOptions _options;

    public TallyHttpClient(HttpClient httpClient, IOptions<TallyMcpOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> PostToTallyAsync(string xml)
    {
        var content = new StringContent(xml, Encoding.UTF8, "application/xml");
        var response = await _httpClient.PostAsync(_options.Tally.GetUrl(), content);
        return await response.Content.ReadAsStringAsync();
    }
}

public static class TallyHttpClientExtensions
{
    private static TallyHttpClient? _instance;
    private static TallyMcpOptions? _options;

    public static void Configure(TallyMcpOptions options)
    {
        _options = options;
        using var httpClient = new HttpClient();
        _instance = new TallyHttpClient(httpClient, Microsoft.Extensions.Options.Options.Create(options));
    }

    public static async Task<string> PostToTallyAsync(string xml)
    {
        if (_instance == null || _options == null)
        {
            // Fallback to default configuration
            _options = new TallyMcpOptions();
            using var httpClient = new HttpClient();
            _instance = new TallyHttpClient(httpClient, Microsoft.Extensions.Options.Options.Create(_options));
        }

        using var client = new HttpClient();
        var content = new StringContent(xml, Encoding.UTF8, "application/xml");
        var response = await client.PostAsync(_options.Tally.GetUrl(), content);
        return await response.Content.ReadAsStringAsync();
    }
}
