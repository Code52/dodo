namespace BoxKite.Models
{
    public class User : PropertyChangedBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _avatar;
        public string Avatar
        {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }
    }
}