namespace RagSharp.Abstractions;

public interface IMemoryStore
{
    /// <summary>
    /// یک قطعه متن و بردار مرتبطش را ذخیره می‌کند.
    /// </summary>
    Task SaveAsync(string text, float[] embedding, string? tag = null);

    /// <summary>
    /// بر اساس بردار جستجو می‌کند و متون مشابه را برمی‌گرداند.
    /// </summary>
    Task<List<string>> SearchAsync(float[] embedding, int topK = 5, string? tagFilter = null);
}
