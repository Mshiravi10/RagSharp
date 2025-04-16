namespace RagSharp.Utils;

public static class TextChunker
{
    /// <summary>
    /// متن را به chunkهایی با اندازه محدود تقسیم می‌کند.
    /// </summary>
    /// <param name="text">متن کامل ورودی</param>
    /// <param name="maxCharsPerChunk">حداکثر تعداد کاراکتر در هر chunk</param>
    public static List<string> Chunk(string text, int maxCharsPerChunk = 500)
    {
        var chunks = new List<string>();
        var currentChunk = new List<string>();
        int currentLength = 0;

        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (currentLength + trimmed.Length > maxCharsPerChunk)
            {
                chunks.Add(string.Join(" ", currentChunk));
                currentChunk.Clear();
                currentLength = 0;
            }

            currentChunk.Add(trimmed);
            currentLength += trimmed.Length;
        }

        if (currentChunk.Count > 0)
        {
            chunks.Add(string.Join(" ", currentChunk));
        }

        return chunks;
    }
}
