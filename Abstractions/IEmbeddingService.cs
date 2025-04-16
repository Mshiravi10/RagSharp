namespace RagSharp.Abstractions;

public interface IEmbeddingService
{
    /// <summary>
    /// یک متن را به بردار تبدیل می‌کند (embedding).
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string text);

    /// <summary>
    /// پاسخ مدل زبانی را برای یک پرامپت کامل برمی‌گرداند.
    /// </summary>
    Task<string> GetCompletionAsync(string prompt, bool isKeyword);
}
