using RagSharp.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;

namespace RagSharp.MemoryStores;

public class QdrantMemoryStore : IMemoryStore
{
    private readonly HttpClient _httpClient;
    private readonly string _collectionName;

    public QdrantMemoryStore(string baseUrl, string collectionName = "rag_data")
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _collectionName = collectionName;
    }

    public async Task SaveAsync(string text, float[] embedding, string? tag = null)
    {
        var payload = new
        {
            points = new[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    vector = embedding,
                    payload = new { text, tag }
                }
            }
        };

        await _httpClient.PutAsJsonAsync($"/collections/{_collectionName}/points?wait=true", payload);
    }

    public async Task<List<string>> SearchAsync(float[] embedding, int topK = 5, string? tagFilter = null)
    {
        var filter = tagFilter != null
            ? new { must = new[] { new { key = "tag", match = new { value = tagFilter } } } }
            : null;

        var request = new
        {
            vector = embedding,
            top = topK,
            with_payload = true,
            filter
        };

        var response = await _httpClient.PostAsJsonAsync($"/collections/{_collectionName}/points/search", request);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var result = new List<string>();
        foreach (var point in doc.RootElement.GetProperty("result").EnumerateArray())
        {
            var text = point.GetProperty("payload").GetProperty("text").GetString();
            if (!string.IsNullOrWhiteSpace(text))
                result.Add(text);
        }

        return result;
    }
}

