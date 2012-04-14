using System;
using BoxKite.Extensions;

namespace BoxKite.Models
{
    public class Tweet : BindableBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private string _avatar;
        public string Avatar
        {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); }
        }

        private DateTime _date;
        public DateTime Time
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public string FriendlyTime
        {
            get { return Time.ToFriendlyText(); }
        }
    }
}
