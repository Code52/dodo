using System;
using BoxKite.Extensions;

namespace BoxKite.Models
{
    public class Tweet : PropertyChangedBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private DateTimeOffset _date;
        public DateTimeOffset Time
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public string FriendlyTime
        {
            get { return Time.ToFriendlyText(); }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private User _user;
        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private User _retweetedBy;
        public User RetweetedBy
        {
            get { return _retweetedBy; }
            set { SetProperty(ref _retweetedBy, value); }
        }
    }
}
