using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    public static class PersianNormalizer
    {
        /// <summary>
        /// نرمال‌سازی متن فارسی: اصلاح حروف عربی، حذف نیم‌فاصله، کشیده و فاصله‌های اضافه
        /// </summary>
        public static string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Trim();

            return text
                .Replace("ي", "ی")                 // عربی به فارسی
                .Replace("ك", "ک")                 // عربی به فارسی
                .Replace("‌", "")                  // نیم‌فاصله
                .Replace("ـ", "")                  // کشیده
                .Replace("\u200c", "")             // Zero-width non-joiner
                .Replace("\r", "")
                .Replace("\n", " ")
                .Replace("  ", " ")                // فاصله‌های اضافه
                .Normalize(NormalizationForm.FormC);
        }
    }
}
