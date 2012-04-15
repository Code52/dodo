using System.Collections.Generic;

namespace BoxKite.Models.Internal
{
    internal class BoundingBox
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }
}