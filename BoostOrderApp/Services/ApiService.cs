using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BoostOrderApp.Models;

namespace BoostOrderApp.Services;

public sealed class ApiService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://cloud.boostorder.com";
    private const string ProductsPath = "/bo-mart/api/v1/wp-json/wc/v1/bo/products";
    private const string Username = "ck_b9e4e281dc7aa5595062207a479090a390304335";
    private const string Password = "cs_95b5c4724a48737ed72daf8314dae9cbc83842ae";

    public ApiService(HttpClient? httpClient = null)
    {
        _http = httpClient ?? new HttpClient { BaseAddress = new Uri(BaseUrl) };
        var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
    }

    public async Task<(List<ProductDto> Items, int Total, int TotalPages)> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var all = new List<ProductDto>();
        int total = 0, totalPages = 1;

        // First request to discover total pages
        var first = await _http.GetAsync($"{ProductsPath}?page=1", cancellationToken);
        first.EnsureSuccessStatusCode();

        total = ReadHeaderInt(first.Headers, "X-WC-Total");
        totalPages = ReadHeaderInt(first.Headers, "X-WC-TotalPages");

        var payload = await first.Content.ReadAsStringAsync(cancellationToken);
        
        var response = JsonSerializer.Deserialize<ProductsResponse>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var pageItems = response?.Products ?? new List<ProductDto>();
        all.AddRange(pageItems);

        // Remaining pages
        for (int page = 2; page <= Math.Max(1, totalPages); page++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var resp = await _http.GetAsync($"{ProductsPath}?page={page}", cancellationToken);
            resp.EnsureSuccessStatusCode();
            var text = await resp.Content.ReadAsStringAsync(cancellationToken);

            var remainingResponse = JsonSerializer.Deserialize<ProductsResponse>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var items = remainingResponse?.Products ?? new List<ProductDto>();
            all.AddRange(items);
        }

        return (all, total, totalPages);
    }

    private static int ReadHeaderInt(HttpResponseHeaders headers, string headerName)
    {
        if (headers.TryGetValues(headerName, out var values))
        {
            if (int.TryParse(values.FirstOrDefault(), out int v))
                return v;
        }
        return 0;
    }
}
