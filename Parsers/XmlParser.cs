using RagSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RagSharp.Parsers
{
    public class XmlParser : IFileParser
    {
        public bool CanParse(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> ParseAsync(string filePath)
        {
            var xmlContent = await File.ReadAllTextAsync(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            return ExtractText(doc.DocumentElement);
        }

        private string ExtractText(XmlNode? node)
        {
            if (node == null) return string.Empty;

            var sb = new System.Text.StringBuilder();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is XmlText textNode)
                    sb.AppendLine(textNode.Value?.Trim());
                else
                    sb.AppendLine(ExtractText(child));
            }

            return sb.ToString();
        }
    }
}
