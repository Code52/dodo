namespace BoxKite.Models.Internal
{
    internal class Tweet
    {
        public string created_at { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public bool truncated { get; set; }
        public object in_reply_to_status_id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public int? in_reply_to_user_id { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public User user { get; set; }
        public Geo geo { get; set; }
        public Coordinates coordinates { get; set; }
        public Place place { get; set; }
        public object contributors { get; set; }
        public int retweet_count { get; set; }
        public Entities entities { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public Tweet retweeted_status { get; set; }
        public bool? possibly_sensitive { get; set; }
    }
}