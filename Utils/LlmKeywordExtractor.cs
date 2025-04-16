using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    /// <summary>
    /// Extracts keywords using an LLM (e.g. LLaMA via Ollama).
    /// </summary>
    public class LlmKeywordExtractor : IKeywordExtractor
    {
        private readonly IEmbeddingService _llm;

        public LlmKeywordExtractor(IEmbeddingService llm)
        {
            _llm = llm;
        }

        public async Task<string[]> ExtractKeywordsAsync(string text, string? context = null)
        {
            var prompt = $"""
        لطفاً بین ۵ تا ۱۰ کلیدواژه مهم از متن زیر استخراج کن.
        متن فارسی است. فقط کلمات یا عبارات کلیدی را با کاما جدا کن. هیچ متن اضافه‌ای ننویس.

        "{text}"
        """;

            var raw = await _llm.GetCompletionAsync(prompt, true);

            var rawKeywords = raw.Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return rawKeywords.Select(k => k.Trim()).Where(k => k.Length > 1).ToArray();
        }
    }
}
