namespace BoxKite.Models.Internal
{
    // ReSharper disable InconsistentNaming

    internal class DM
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


