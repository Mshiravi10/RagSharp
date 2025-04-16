using RagSharp.Abstractions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RagSharp.Embedding;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _embeddingModel;
    private readonly string _completionModel;
    private readonly string _keywordModel;

    public OllamaEmbeddingService(string baseUrl, string embeddingModel, string completionModel, string keywordModel)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _embeddingModel = embeddingModel;
        _completionModel = completionModel;
        _keywordModel = keywordModel;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var request = new
        {
            model = _embeddingModel,
            prompt = text,
            options = new { embedding_only = true }
        };

        var response = await _httpClient.PostAsJsonAsync("/api/embeddings", request);
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement
                  .GetProperty("embedding")
                  .EnumerateArray()
                  .Select(x => x.GetSingle())
                  .ToArray();
    }

    public async Task<string> GetCompletionAsync(string prompt, bool isKeyword)
    {
        var request = new
        {
            model = isKeyword ? _keywordModel : _completionModel,
            prompt = prompt,
            stream = true
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var fullResponse = new StringBuilder();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("response", out var part))
                {
                    fullResponse.Append(part.GetString());
                }
            }
            catch (JsonException)
            {
            }
        }

        return fullResponse.ToString().Trim();
    }
}

