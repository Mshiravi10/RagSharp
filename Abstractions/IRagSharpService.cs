using RagSharp.Core;

namespace RagSharp.Abstractions;

public interface IRagSharpService
{
    /// <summary>
    /// فایل را پردازش کرده، متن را استخراج و به حافظه برداری اضافه می‌کند.
    /// </summary>
    Task IngestDocumentAsync(string filePath, string? tag = null);

    /// <summary>
    /// یک سؤال را گرفته و بر اساس حافظه، پاسخ مناسب از مدل LLM می‌گیرد.
    /// </summary>
    Task<string> AskAsync(string question);

    /// <summary>
    /// پاسخ را با استفاده از اطلاعات موجود در حافظه و مدل LLM به‌دست می‌آورد.
    ///  </summary>
    Task<GroundedAnswer> AskWithGroundingAsync(string question);

    /// <summary>
    /// تمام فایل‌های موجود در مسیر مشخص‌شده را پردازش کرده و متن استخراج‌شده را در حافظه ذخیره می‌کند.
    /// </summary>
    Task IngestDirectoryAsync(string folderPath, string? tag = null);
}
