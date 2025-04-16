using RagSharp.Abstractions;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RagSharp.Embedding;

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public OpenAIEmbeddingService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<float[]> GenerateEmbeddingAsync(string input)
    {
        var requestBody = new
        {
            input = input,
            model = "text-embedding-ada-002"
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using JsonDocument json = JsonDocument.Parse(responseString);
        var embedding = json.RootElement.GetProperty("data")[0].GetProperty("embedding");

        int length = embedding.GetArrayLength();
        float[] result = new float[length];
        int index = 0;
        foreach (var number in embedding.EnumerateArray())
        {
            result[index++] = number.GetSingle();
        }

        return result;
    }

    public async Task<string> GetCompletionAsync(string prompt, bool isKeyword)
    {
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = prompt }
        },
            max_tokens = 512,
            temperature = 0.7
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI completion failed: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using JsonDocument json = JsonDocument.Parse(responseString);

        var reply = json.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return reply?.Trim() ?? string.Empty;
    }

}
