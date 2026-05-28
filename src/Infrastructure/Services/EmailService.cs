using System.Text;
using System.Text.Json;

using Application.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory = null!;
    public HttpClient _httpClient { get; set; }

    private readonly IConfiguration _configuration;


    public EmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {

        _httpClientFactory = httpClientFactory;
        string? httpClientName = "brevoClient";
        _httpClient = _httpClientFactory.CreateClient(httpClientName);
        _configuration = configuration;
    }

    public async Task EnviarEmailAsync(string to, string subject, string html)
    {
        var body = new
        {
            sender = new
            {
                name = "EntreTodos",
                email = _configuration["Brevo:From"]
            },

            to = new[]
            {
                new { email = to }
            },

            subject,

            htmlContent = html
        };

        var json =
            JsonSerializer.Serialize(body);

        var request =
            new HttpRequestMessage(HttpMethod.Post, "smtp/email");

        request.Headers.Add("api-key", _configuration["Brevo:ApiKey"]);

        request.Content =
            new StringContent(json, Encoding.UTF8, "application/json");

        var response =
            await _httpClient.SendAsync(request);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseBody);

        response.EnsureSuccessStatusCode();
    }
}