namespace BoxKite.Models
{
    public class TwitterCredentials
    {
        static TwitterCredentials _null = new TwitterCredentials { Valid = false };

        public bool Valid { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }

        public static TwitterCredentials Null
        {
            get { return _null; }
            set { _null = value; }
        }
    }
}
