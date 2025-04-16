using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    public class LlmContextCompressor : IContextCompressor
    {
        private readonly IEmbeddingService _llm;

        public LlmContextCompressor(IEmbeddingService llm)
        {
            _llm = llm;
        }

        public async Task<string> CompressAsync(string[] chunks, string question)
        {
            var sb = new StringBuilder();
            foreach (var (chunk, i) in chunks.Select((c, i) => (c, i)))
            {
                sb.AppendLine($"متن {i + 1}:\n{chunk.Trim()}\n");
            }

            var prompt = $"""
        ما مجموعه‌ای از بخش‌های متنی داریم که ممکنه به سوال زیر مرتبط باشن.
        سوال: "{question}"

        لطفاً این بخش‌ها را خلاصه کن و فقط اطلاعات مهم و مربوط به سوال را نگه‌دار.
        متن نهایی باید کوتاه، دقیق و پوشش‌دهنده اطلاعات اصلی باشد.

        متن‌ها:
        {sb}
        """;

            return await _llm.GetCompletionAsync(prompt, isKeyword: true); // مدل فشرده‌ساز می‌تونه جدا باشه
        }
    }
}
