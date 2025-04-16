using HtmlAgilityPack;
using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Parsers
{
    public class HtmlParser : IFileParser
    {
        public bool CanParse(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".html", StringComparison.OrdinalIgnoreCase) ||
                   Path.GetExtension(filePath).Equals(".htm", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> ParseAsync(string filePath)
        {
            var html = await File.ReadAllTextAsync(filePath);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var text = doc.DocumentNode
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText.Trim())
                .ToList();

            return string.Join(" ", text);
        }
    }
}
