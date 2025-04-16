using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Abstractions
{
    public interface IQueryExpander
    {
        Task<string> ExpandAsync(string question);
    }
}
