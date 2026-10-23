using RagSharp.Utils;
using Xunit;

namespace RagSharp.Tests;

public sealed class TextUtilitiesTests
{
    [Theory]
    [InlineData("این یک متن فارسی است", "fa")]
    [InlineData("This is English text", "en")]
    [InlineData("متن فارسی with English", "multi")]
    [InlineData("12345", "unknown")]
    public void DetectLanguage_ReturnsExpectedLanguage(string text, string expected)
    {
        Assert.Equal(expected, LanguageHelper.DetectDominantLanguage(text));
    }

    [Fact]
    public void Normalize_ConvertsArabicLettersAndCollapsesWhitespace()
    {
        const string input = "كتاب\u200cهاي\n  خوب";

        Assert.Equal("کتابهای خوب", PersianNormalizer.Normalize(input));
    }

    [Fact]
    public void Chunk_RespectsWordBoundariesAndMaximumLength()
    {
        var chunks = TextChunker.Chunk("alpha beta gamma delta", 10);

        Assert.Equal(["alpha beta", "gamma", "delta"], chunks);
        Assert.All(chunks, chunk => Assert.InRange(chunk.Length, 1, 10));
    }

    [Fact]
    public void Chunk_SplitsWordsThatExceedTheMaximumLength()
    {
        Assert.Equal(["abcd", "efgh", "ijk"], TextChunker.Chunk("abcdefghijk", 4));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Chunk_RejectsInvalidMaximumLength(int maxLength)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TextChunker.Chunk("text", maxLength));
    }
}
