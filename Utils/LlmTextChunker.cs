using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    public class LlmTextChunker
    {
        private readonly IEmbeddingService _llm;

        public LlmTextChunker(IEmbeddingService llm)
        {
            _llm = llm;
        }

        public async Task<List<string>> ChunkAsync(string longText)
        {
            var prompt = $"""
        متن زیر بسیار بلند است. لطفاً آن را به بخش‌هایی تقسیم کن که:
        - هر بخش حداکثر ۴۰۰ تا ۵۰۰ کاراکتر باشد
        - معنای هر بخش مستقل باشد
        - متن وسط جمله یا پاراگراف قطع نشود

        لطفاً هر بخش را با علامت `---` جدا کن.

        متن:
        {longText}
        """;

            var response = await _llm.GetCompletionAsync(prompt, isKeyword: true);
            var chunks = response.Split("---", StringSplitOptions.RemoveEmptyEntries)
                                 .Select(x => x.Trim())
                                 .Where(x => x.Length > 100)
                                 .ToList();

            return chunks;
        }
    }
}
