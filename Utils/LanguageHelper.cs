using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Utils
{
    public static class LanguageHelper
    {
        public static string DetectDominantLanguage(string text)
        {
            int faCount = text.Count(c => c >= 0x0600 && c <= 0x06FF);
            int enCount = text.Count(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));

            if (faCount == 0 && enCount == 0)
                return "unknown";

            if (faCount > 0 && enCount > 0)
                return "multi"; // متن دوزبانه

            return faCount > enCount ? "fa" : "en";
        }
    }
}
