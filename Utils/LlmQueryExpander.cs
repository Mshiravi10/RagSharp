using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    public class LlmQueryExpander : IQueryExpander
    {
        private readonly IEmbeddingService _llm;

        public LlmQueryExpander(IEmbeddingService llm)
        {
            _llm = llm;
        }

        public async Task<string> ExpandAsync(string question)
        {
            var prompt = $"""
        لطفاً سوال زیر را بازنویسی کن و گسترش بده تا واضح‌تر و دقیق‌تر باشد.
        فقط سوال جدید را چاپ کن. از حشو، توضیح یا جملات اضافه خودداری کن.

        سوال:
        "{question}"
        """;

            var response = await _llm.GetCompletionAsync(prompt, isKeyword: true); // می‌تونه مدل مخصوص expansion باشه
            return response.Trim();
        }
    }
}
