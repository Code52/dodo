using System.Runtime.Serialization;

namespace BoxKite.Models
{
    [DataContract]
    public class TwitterCredentials
    {
        [DataMember]
        public bool Valid { get; set; }
        [DataMember]
        public string ConsumerKey { get; set; }
        [DataMember]
        public string ConsumerSecret { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string TokenSecret { get; set; }

        [IgnoreDataMember]
        static TwitterCredentials _null = new TwitterCredentials { Valid = false };

        [IgnoreDataMember]
        public static TwitterCredentials Null
        {
            get { return _null; }
        }
    }
}
