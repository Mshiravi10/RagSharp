using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            text = text
                .Replace("ي", "ی")                 // عربی به فارسی
                .Replace("ك", "ک")                 // عربی به فارسی
                .Replace("‌", "")                  // نیم‌فاصله
                .Replace("ـ", "")                  // کشیده
                .Replace("\u200c", "")             // Zero-width non-joiner
                .Normalize(NormalizationForm.FormC);

            return Regex.Replace(text, @"\s+", " ").Trim();
        }
    }
}
