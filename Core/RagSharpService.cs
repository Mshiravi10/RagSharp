using RagSharp.Abstractions;
using RagSharp.Core;
using RagSharp.Utils;
using System.Text;

namespace RagSharp.Core;

public class RagSharpService : IRagSharpService
{
    private readonly IEmbeddingService _embedding;
    private readonly IMemoryStore _memoryStore;
    private readonly IEnumerable<IFileParser> _parsers;
    private readonly IKeywordExtractor? _keywordExtractor;
    private readonly IContextCompressor? _contextCompressor;
    private readonly IQueryExpander? _queryExpander;


    public RagSharpService(RagSharpOptions options,
                           IEmbeddingService embedding,
                           IMemoryStore memoryStore,
                           IEnumerable<IFileParser> parsers)
    {
        _embedding = embedding;
        _memoryStore = memoryStore;
        _parsers = parsers;
        _keywordExtractor = options.KeywordExtractor;
        _contextCompressor = options.ContextCompressor;
        _queryExpander = options.QueryExpander;
    }

    public async Task IngestDocumentAsync(string filePath, string? tag = null)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(filePath));
        if (parser == null) return;

        var text = await parser.ParseAsync(filePath);
        var lang = LanguageHelper.DetectDominantLanguage(text);
        var smartChunker = new SmartChunker(_embedding);
        var chunks = await smartChunker.ChunkAsync(text);


        foreach (var chunk in chunks)
        {
            var embedding = await _embedding.GenerateEmbeddingAsync(chunk);

            if (lang == "multi")
            {
                await _memoryStore.SaveAsync(chunk, embedding, "fa");
                await _memoryStore.SaveAsync(chunk, embedding, "en");
            }
            else
            {
                await _memoryStore.SaveAsync(chunk, embedding, lang);
            }
        }
    }

    public async Task IngestDirectoryAsync(string folderPath, string? tag = null)
    {
        var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                             .Where(f => _parsers.Any(p => p.CanParse(f)));

        foreach (var file in files)
        {
            try
            {
                await IngestDocumentAsync(file, tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ خطا در پردازش فایل {file}: {ex.Message}");
            }
        }
    }

    public async Task<string> AskAsync(string question)
    {
        if (_queryExpander != null)
        {
            question = await _queryExpander.ExpandAsync(question);
        }

        var lang = LanguageHelper.DetectDominantLanguage(question);
        var normalizedQuestion = PersianNormalizer.Normalize(question);
        var questionEmbedding = await _embedding.GenerateEmbeddingAsync(normalizedQuestion);

        string[] questionKeywords = Array.Empty<string>();
        if (_keywordExtractor != null)
            questionKeywords = await _keywordExtractor.ExtractKeywordsAsync(normalizedQuestion);

        // بازیابی اولیه از حافظه
        var topChunks = lang == "multi"
            ? (await _memoryStore.SearchAsync(questionEmbedding, 10, "fa"))
                .Concat(await _memoryStore.SearchAsync(questionEmbedding, 10, "en"))
                .Distinct()
                .ToList()
            : await _memoryStore.SearchAsync(questionEmbedding, 20, lang);

        // نمره‌دهی
        var scored = new List<ScoredChunk>();
        foreach (var chunk in topChunks)
        {
            var chunkEmbedding = await _embedding.GenerateEmbeddingAsync(chunk);
            float semanticScore = CalculateCosineSimilarity(questionEmbedding, chunkEmbedding);

            float keywordScore = 0;
            if (_keywordExtractor != null)
            {
                var chunkKeywords = await _keywordExtractor.ExtractKeywordsAsync(chunk);
                keywordScore = CalculateKeywordOverlap(questionKeywords, chunkKeywords);
            }

            scored.Add(new ScoredChunk
            {
                Text = chunk,
                SemanticScore = semanticScore,
                KeywordScore = keywordScore
            });
        }

        var bestChunks = scored
            .OrderByDescending(x => x.FinalScore)
            .Take(7) // ۷ تا chunk بالا
            .Select(x => x.Text)
            .ToArray();

        // 🎯 Context Compression اینجاست!
        string context;
        if (_contextCompressor != null)
        {
            context = await _contextCompressor.CompressAsync(bestChunks, question);
        }
        else
        {
            context = string.Join("\n", bestChunks);
        }

        // ساخت prompt نهایی
        var prompt = $"""
    بر اساس اطلاعات زیر پاسخ بده:
    {context}

    سوال:
    {question}

    لطفاً پاسخ را دقیق، واضح و کوتاه ارائه کن.
    """;

        return await _embedding.GetCompletionAsync(prompt, false);
    }


    public async Task<GroundedAnswer> AskWithGroundingAsync(string question)
    {
        if (_queryExpander != null)
        {
            question = await _queryExpander.ExpandAsync(question);
        }

        var lang = LanguageHelper.DetectDominantLanguage(question);
        var normalizedQuestion = PersianNormalizer.Normalize(question);
        var questionEmbedding = await _embedding.GenerateEmbeddingAsync(normalizedQuestion);

        string[] questionKeywords = Array.Empty<string>();
        if (_keywordExtractor != null)
            questionKeywords = await _keywordExtractor.ExtractKeywordsAsync(normalizedQuestion);

        var topChunks = lang == "multi"
            ? (await _memoryStore.SearchAsync(questionEmbedding, 10, "fa"))
                .Concat(await _memoryStore.SearchAsync(questionEmbedding, 10, "en"))
                .Distinct()
                .ToList()
            : await _memoryStore.SearchAsync(questionEmbedding, 20, lang);

        var scored = new List<ScoredChunk>();
        foreach (var chunk in topChunks)
        {
            var chunkEmbedding = await _embedding.GenerateEmbeddingAsync(chunk);
            float semanticScore = CalculateCosineSimilarity(questionEmbedding, chunkEmbedding);

            float keywordScore = 0;
            if (_keywordExtractor != null)
            {
                var chunkKeywords = await _keywordExtractor.ExtractKeywordsAsync(chunk);
                keywordScore = CalculateKeywordOverlap(questionKeywords, chunkKeywords);
            }

            scored.Add(new ScoredChunk
            {
                Text = chunk,
                SemanticScore = semanticScore,
                KeywordScore = keywordScore
            });
        }

        var bestChunks = scored
            .OrderByDescending(x => x.FinalScore)
            .Take(7)
            .ToList();

        var bestTexts = bestChunks.Select(x => x.Text).ToArray();

        string context;
        if (_contextCompressor != null)
        {
            context = await _contextCompressor.CompressAsync(bestTexts, question);
        }
        else
        {
            context = string.Join("\n", bestTexts);
        }

        var prompt = $"""
    بر اساس اطلاعات زیر پاسخ بده:
    {context}

    سوال:
    {question}

    لطفاً پاسخ را دقیق، واضح و کوتاه ارائه کن.
    """;

        var answer = await _embedding.GetCompletionAsync(prompt, false);

        return new GroundedAnswer
        {
            Answer = answer,
            Sources = bestChunks.Select(x => new GroundedChunk
            {
                Text = x.Text,
                Score = x.FinalScore
            }).ToList()
        };
    }



    private static float CalculateKeywordOverlap(string[] q, string[] c)
    {
        if (q.Length == 0 || c.Length == 0) return 0f;
        var overlap = q.Intersect(c, StringComparer.OrdinalIgnoreCase).Count();
        return (float)overlap / q.Length;
    }

    private static float CalculateCosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0f;
        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return (float)(dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-8));
    }
}
