using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using RagSharp.Abstractions;

namespace RagSharp.Parsers;

public class WordParser : IFileParser
{
    public bool CanParse(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".docx", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> ParseAsync(string filePath)
    {
        var text = new System.Text.StringBuilder();

        await Task.Run(() =>
        {
            using var doc = WordprocessingDocument.Open(filePath, false);
            var body = doc.MainDocumentPart?.Document?.Body;

            if (body != null)
            {
                foreach (var para in body.Elements<Paragraph>())
                {
                    text.AppendLine(para.InnerText);
                }
            }
        });

        return text.ToString();
    }
}
