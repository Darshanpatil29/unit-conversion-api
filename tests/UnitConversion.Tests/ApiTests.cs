using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Tests;

public class ApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Convert_returns_result()
    {
        var json = await _client.GetFromJsonAsync<JsonElement>("/api/convert?value=100&from=celsius&to=fahrenheit");

        Assert.Equal(212, json.GetProperty("result").GetDouble());
        Assert.Equal("temperature", json.GetProperty("category").GetString());
    }

    [Theory]
    [InlineData("/api/convert?value=1&from=m&to=kg")]   // incompatible units
    [InlineData("/api/convert?value=1&from=xyz&to=m")]  // unknown unit
    [InlineData("/api/convert?from=m&to=ft")]           // missing value
    [InlineData("/api/convert?value=abc&from=m&to=ft")] // value is not a number
    public async Task Convert_returns_400_for_bad_requests(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Units_can_be_filtered_by_category()
    {
        var units = await _client.GetFromJsonAsync<JsonElement[]>("/api/units?category=temperature");

        Assert.NotNull(units);
        Assert.Equal(3, units.Length);
    }
}
