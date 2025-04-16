using RagSharp.Abstractions;

namespace RagSharp.Core;

public class RagSharpOptions
{
    /// <summary>
    /// سرویسی برای تولید بردار (embedding)
    /// </summary>
    public IEmbeddingService? EmbeddingService { get; set; }

    /// <summary>
    /// حافظه‌ای برای ذخیره‌سازی بردارها (Qdrant, Redis, ES)
    /// </summary>
    public IMemoryStore? MemoryStore { get; set; }

    /// <summary>
    /// لیست پارسرهایی که فایل‌ها رو به متن تبدیل می‌کنن (PDF, DOCX, HTML...)
    /// </summary>
    public List<IFileParser> FileParsers { get; set; } = new();

    /// <summary>
    /// مشخص کننده استخراج کلیدواژه‌ها (مثل TF-IDF یا RAKE)
    public IKeywordExtractor? KeywordExtractor { get; set; }

    /// <summary>
    /// برای فشرده‌سازی متن‌ها استفاده می‌شه
    /// </summary>
    public IContextCompressor? ContextCompressor { get; set; }


    /// <summary>
    /// برای گسترش پرسش‌ها استفاده می‌شه
    ///  </summary>
    public IQueryExpander? QueryExpander { get; set; }



    /// <summary>
    /// حداکثر طول هر chunk (برای TextChunker)
    /// </summary>
    public int ChunkSize { get; set; } = 500;
}
