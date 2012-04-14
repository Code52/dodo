namespace BoxKite.Models
{
    public class DirectMessage : Tweet
    {
        private string _recipient;
        public string Recipient
        {
            get { return _recipient; }
            set { SetProperty(ref _recipient, value); }
        }
    }
}