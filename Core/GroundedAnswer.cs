using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Core
{
    public class GroundedAnswer
    {
        public string Answer { get; set; } = string.Empty;
        public List<GroundedChunk> Sources { get; set; } = new();
    }

    public class GroundedChunk
    {
        public string Text { get; set; } = string.Empty;
        public float Score { get; set; }
    }
}
