namespace RagSharp.Abstractions;

public interface IFileParser
{
    /// <summary>
    /// بررسی می‌کند که آیا این parser می‌تواند فایل مورد نظر را بخواند یا نه.
    /// </summary>
    bool CanParse(string filePath);

    /// <summary>
    /// فایل را خوانده و متن استخراج‌شده را بازمی‌گرداند.
    /// </summary>
    Task<string> ParseAsync(string filePath);
}
