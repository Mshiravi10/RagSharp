using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Abstractions
{
    public interface IContextCompressor
    {
        Task<string> CompressAsync(string[] chunks, string question);
    }
}
