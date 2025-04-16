using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Parsers
{
    public class TextParser : IFileParser
    {
        public bool CanParse(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> ParseAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
