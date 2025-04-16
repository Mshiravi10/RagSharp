using Nest;
using RagSharp.Abstractions;

namespace RagSharp.MemoryStores;

public class ElasticsearchMemoryStore : IMemoryStore
{
    private readonly IElasticClient _client;
    private readonly string _indexName = "rag_entries";

    public ElasticsearchMemoryStore(string elasticUrl)
    {
        var settings = new ConnectionSettings(new Uri(elasticUrl))
            .DefaultIndex(_indexName);
        _client = new ElasticClient(settings);

        if (!_client.Indices.Exists(_indexName).Exists)
        {
            _client.Indices.Create(_indexName, c => c
                .Map<RagEntry>(m => m
                    .AutoMap()
                )
            );
        }
    }

    public async Task SaveAsync(string text, float[] embedding, string? tag = null)
    {
        var doc = new RagEntry
        {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            Tag = tag,
            Vector = embedding
        };

        await _client.IndexDocumentAsync(doc);
    }

    public async Task<List<string>> SearchAsync(float[] embedding, int topK = 5, string? tagFilter = null)
    {
        var query = new Func<QueryContainerDescriptor<RagEntry>, QueryContainer>(q =>
        {
            QueryContainer baseQuery = q.MatchAll();

            if (!string.IsNullOrWhiteSpace(tagFilter))
            {
                baseQuery &= q.Term(t => t.Field(f => f.Tag).Value(tagFilter));
            }

            return q.ScriptScore(ss => ss
                .Query(_ => baseQuery)
                .Script(sc => sc.Source(@"
                    cosineSimilarity(params.query_vector, 'vector') + 1.0
                ").Params(p => p.Add("query_vector", embedding)))
            );
        });

        var search = await _client.SearchAsync<RagEntry>(s => s
            .Size(topK)
            .Query(query)
        );

        return search.Documents.Select(d => d.Text).ToList();
    }

    private class RagEntry
    {
        public string Id { get; set; } = default!;
        public string Text { get; set; } = default!;
        public string? Tag { get; set; }
        public float[] Vector { get; set; } = Array.Empty<float>();
    }
}
