using RagSharp.Abstractions;
using UglyToad.PdfPig;

namespace RagSharp.Parsers;

public class PdfParser : IFileParser
{
    public bool CanParse(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> ParseAsync(string filePath)
    {
        var text = new System.Text.StringBuilder();

        await Task.Run(() =>
        {
            using var document = PdfDocument.Open(filePath);
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
        });

        return text.ToString();
    }
}
