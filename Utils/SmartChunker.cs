using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    /// <summary>
    /// SmartChunker به صورت خودکار تصمیم می‌گیرد متن را با LLM یا روش سنتی chunk کند.
    /// </summary>
    public class SmartChunker
    {
        private readonly IEmbeddingService _llm;
        private readonly int _charLimitForLLM;

        public SmartChunker(IEmbeddingService llm, int charLimitForLLM = 800)
        {
            _llm = llm;
            _charLimitForLLM = charLimitForLLM;
        }

        public async Task<List<string>> ChunkAsync(string text, int fallbackMaxLength = 500)
        {
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();

            foreach (var para in paragraphs)
            {
                var cleanPara = para.Trim();
                if (cleanPara.Length == 0)
                    continue;

                if (cleanPara.Length > _charLimitForLLM)
                {
                    var smartChunks = await ChunkWithLLM(cleanPara);
                    chunks.AddRange(smartChunks);
                }
                else
                {
                    var fallbackChunks = TextChunker.Chunk(cleanPara, fallbackMaxLength);
                    chunks.AddRange(fallbackChunks);
                }
            }

            return chunks;
        }

        private async Task<List<string>> ChunkWithLLM(string longText)
        {
            var prompt = $"""
        متن زیر طولانی است. لطفاً آن را به بخش‌هایی تقسیم کن که:
        - هر بخش حداکثر ۵۰۰ کاراکتر باشد
        - معنای هر بخش مستقل باشد و وسط جمله قطع نشود
        - فقط متن را برگردان، هر بخش را با `---` جدا کن

        متن:
        {longText}
        """;

            var response = await _llm.GetCompletionAsync(prompt, isKeyword: true);

            return response.Split("---", StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim())
                           .Where(x => x.Length > 100)
                           .ToList();
        }
    }
}
