using System.Collections.Generic;

namespace BoxKite.Models.Internal
{
    internal class Entities
    {
        public List<object> hashtags { get; set; }
        public List<object> urls { get; set; }
        public List<UserMention> user_mentions { get; set; }
    }
}