using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSharp.Core
{
    public class ScoredChunk
    {
        public string Text { get; set; } = string.Empty;
        public float SemanticScore { get; set; }
        public float KeywordScore { get; set; }
        public float FinalScore => SemanticScore * 0.7f + KeywordScore * 0.3f;
    }
}
