# 🧠 RagSharp

**RagSharp** یک موتور Retrieval-Augmented Generation (RAG) پیشرفته با معماری ماژولار است که با .NET توسعه داده شده  
و از زبان فارسی، انگلیسی، و پردازش هوشمند چندمرحله‌ای پشتیبانی می‌کند.

---

## ✨ قابلیت‌ها

- 🔍 **جستجوی معنایی** با استفاده از Embedding و Vector DB (Ollama / OpenAI + Redis/Qdrant/Elastic)
- 🧠 **استخراج کلیدواژه، گسترش سوال و فشرده‌سازی context** با کمک LLM
- 📚 **خواندن فایل‌ها** در فرمت‌های مختلف: PDF, Word, HTML, Markdown, Text, XML
- 📌 **Answer Grounding** برای نمایش منابع پاسخ
- 🇮🇷 **پشتیبانی کامل از زبان فارسی و انگلیسی**
- ⚙️ معماری قابل توسعه و تزریق سرویس‌های سفارشی

---

## 🚀 نصب

```bash
dotnet add package RagSharp
```

---

## ⚡ استفاده سریع

```csharp
services.AddRagSharp(options =>
{
    options.EmbeddingService = new OllamaEmbeddingService(
        "http://localhost:11434",
        "bge-m3",             // مدل بردارسازی
        "llama3",             // مدل تولید پاسخ
        "llama3-keyword"      // مدل استخراج کلیدواژه
    );

    options.MemoryStore = new QdrantMemoryStore("http://localhost:6333");

    options.FileParsers.Add(new PdfParser());
    options.FileParsers.Add(new WordParser());
    options.FileParsers.Add(new HtmlParser());
    options.FileParsers.Add(new TextParser());

    options.QueryExpander     = new LlmQueryExpander(options.EmbeddingService);
    options.KeywordExtractor  = new LlmKeywordExtractor(options.EmbeddingService);
    options.ContextCompressor = new LlmContextCompressor(options.EmbeddingService);

    options.ChunkSize = 500;
});
```

---

## 🧪 مثال پرسش

```csharp
var rag = host.Services.GetRequiredService<IRagSharpService>();

var result = await rag.AskWithGroundingAsync("شرایط فسخ قرارداد چیست؟");

Console.WriteLine("پاسخ:");
Console.WriteLine(result.Answer);

Console.WriteLine("\nمنابع:");
foreach (var chunk in result.Sources)
{
    Console.WriteLine($"[Score: {chunk.Score:F2}] {chunk.Text.Substring(0, Math.Min(100, chunk.Text.Length))}...");
}
```

---

## 🧱 ساختار پروژه

```bash
RagSharp/
│
├── Abstractions/         # اینترفیس‌های اصلی
├── Core/                 # سرویس اصلی RagSharpService و مدل‌ها
├── Embedding/            # اتصال به Ollama و OpenAI برای بردارسازی
├── MemoryStores/         # پیاده‌سازی Qdrant، Redis، Elastic
├── Parsers/              # پردازش فایل‌ها (PDF, Word, HTML, Text, Markdown, XML)
├── Utils/                # ابزارهای تکمیلی (Normalizer, Chunker, Compressor, ...)
├── Extensions/           # DI Extensions برای AddRagSharp
└── RagSharp.csproj
```

---

## 📜 License

RagSharp is licensed under the **Apache License 2.0**  
See [`LICENSE`](./LICENSE) for more information.
