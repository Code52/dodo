using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Models
{
    // ReSharper disable InconsistentNaming

    public class Metadata
    {
        public string result_type { get; set; }
    }

    public class Result
    {
        public string created_at { get; set; }
        public string from_user { get; set; }
        public int from_user_id { get; set; }
        public string from_user_id_str { get; set; }
        public string from_user_name { get; set; }
        public object geo { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }
        public string iso_language_code { get; set; }
        public Metadata metadata { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string source { get; set; }
        public string text { get; set; }
        public object to_user { get; set; }
        public object to_user_id { get; set; }
        public object to_user_id_str { get; set; }
        public object to_user_name { get; set; }
    }

    public class SearchResponse
    {
        public double completed_in { get; set; }
        public long max_id { get; set; }
        public string max_id_str { get; set; }
        public string next_page { get; set; }
        public int page { get; set; }
        public string query { get; set; }
        public string refresh_url { get; set; }
        public List<Result> results { get; set; }
        public int results_per_page { get; set; }
        public int since_id { get; set; }
        public string since_id_str { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        [JsonProperty("protected")]
        public bool is_protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public int? utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool show_all_inline_media { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public bool? following { get; set; }
        [JsonIgnore]
        public bool follow_request_sent { get; set; }
        public object notifications { get; set; }
    }

    public class Geo
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Coordinates
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class BoundingBox
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class Attributes
    {
    }

    public class Place
    {
        public string id { get; set; }
        public string url { get; set; }
        public string place_type { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string country_code { get; set; }
        public string country { get; set; }
        public BoundingBox bounding_box { get; set; }
        public Attributes attributes { get; set; }
    }

    public class UserMention
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
    }

    public class Entities
    {
        public List<object> hashtags { get; set; }
        public List<object> urls { get; set; }
        public List<UserMention> user_mentions { get; set; }
    }

    public class Mention
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
        public bool? possibly_sensitive { get; set; }
    }

    public class DM
    {
        public string created_at { get; set; }
        public string sender_screen_name { get; set; }
        public User sender { get; set; }
        public string text { get; set; }
        public string recipient_screen_name { get; set; }
        public long id { get; set; }
        public User recipient { get; set; }
        public long recipient_id { get; set; }
        public long sender_id { get; set; }
    }
    // ReSharper restore InconsistentNaming
}


