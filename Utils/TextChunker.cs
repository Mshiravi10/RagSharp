using System.Text;
using System.Text.RegularExpressions;

namespace RagSharp.Utils;

public static class TextChunker
{
    /// <summary>
    /// متن را بدون تولید قطعه خالی به بخش هایی با اندازه محدود تقسیم می کند
    /// </summary>
    public static List<string> Chunk(string text, int maxCharsPerChunk = 500)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCharsPerChunk);

        if (string.IsNullOrWhiteSpace(text))
            return [];

        var chunks = new List<string>();
        var currentChunk = new StringBuilder(maxCharsPerChunk);

        void FlushCurrentChunk()
        {
            if (currentChunk.Length == 0)
                return;

            chunks.Add(currentChunk.ToString());
            currentChunk.Clear();
        }

        foreach (var word in Regex.Split(text.Trim(), @"\s+"))
        {
            if (word.Length > maxCharsPerChunk)
            {
                FlushCurrentChunk();

                for (var offset = 0; offset < word.Length; offset += maxCharsPerChunk)
                {
                    var length = Math.Min(maxCharsPerChunk, word.Length - offset);
                    chunks.Add(word.Substring(offset, length));
                }

                continue;
            }

            var requiredLength = currentChunk.Length == 0 ? word.Length : word.Length + 1;
            if (currentChunk.Length + requiredLength > maxCharsPerChunk)
                FlushCurrentChunk();

            if (currentChunk.Length > 0)
                currentChunk.Append(' ');

            currentChunk.Append(word);
        }

        FlushCurrentChunk();
        return chunks;
    }
}
