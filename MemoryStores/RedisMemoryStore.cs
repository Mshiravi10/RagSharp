using RagSharp.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace RagSharp.MemoryStores;

public class RedisMemoryStore : IMemoryStore
{
    private readonly IDatabase _db;
    private readonly string _prefix = "rag_memory:";

    public RedisMemoryStore(string redisConnectionString)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _db = redis.GetDatabase();
    }

    public async Task SaveAsync(string text, float[] embedding, string? tag = null)
    {
        var id = Guid.NewGuid().ToString();
        var entry = new RagEntry
        {
            Id = id,
            Text = text,
            Embedding = embedding,
            Tag = tag
        };

        string json = JsonSerializer.Serialize(entry);
        await _db.StringSetAsync(_prefix + id, json);
    }

    public async Task<List<string>> SearchAsync(float[] embedding, int topK = 5, string? tagFilter = null)
    {
        var server = GetServer();
        var keys = server.Keys(pattern: _prefix + "*").ToList();

        var entries = new List<RagEntry>();
        foreach (var key in keys)
        {
            var json = await _db.StringGetAsync(key);
            if (!json.IsNullOrEmpty)
            {
                var entry = JsonSerializer.Deserialize<RagEntry>(json!)!;

                // اعمال فیلتر بر اساس تگ
                if (!string.IsNullOrWhiteSpace(tagFilter) &&
                    !string.Equals(entry.Tag, tagFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                entry.Score = L2Distance(entry.Embedding, embedding);
                entries.Add(entry);
            }
        }

        return entries
            .OrderBy(e => e.Score)
            .Where(e => !string.IsNullOrEmpty(e.Text))
            .Take(topK)
            .Select(e => e.Text)
            .ToList();
    }

    private static double L2Distance(float[] a, float[] b)
    {
        return Math.Sqrt(a.Zip(b, (x, y) => Math.Pow(x - y, 2)).Sum());
    }

    private IServer GetServer()
    {
        var endpoints = _db.Multiplexer.GetEndPoints();
        return _db.Multiplexer.GetServer(endpoints[0]);
    }

    private class RagEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public string? Tag { get; set; }
        public double Score { get; set; }
    }
}
