using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public static class TallyHttpClient
{
    public static async Task<string> PostToTallyAsync(string xml)
    {
        using var client = new HttpClient();
        var content = new StringContent(xml, Encoding.UTF8, "application/xml");
        var response = await client.PostAsync("http://localhost:9000", content);
        return await response.Content.ReadAsStringAsync();
    }
}
