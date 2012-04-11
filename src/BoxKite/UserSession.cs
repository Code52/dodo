using System;
using System.Threading.Tasks;
using BoxKite.Models;

namespace BoxKite
{
    public class UserSession : IUserSession
    {
        private readonly TwitterCredentials _credentials;

        public UserSession(TwitterCredentials credentials)
        {
            _credentials = credentials;
        }

        public IObservable<Tweet> GetTimeline()
        {
            throw new NotImplementedException();
        }

        public IObservable<Tweet> GetMentions()
        {
            throw new NotImplementedException();
        }

        public IObservable<Tweet> GetDirectMessages()
        {
            throw new NotImplementedException();
        }

        public IObservable<Tweet> GetRetweets()
        {
            throw new NotImplementedException();
        }

        public Task<Tweet> UpdateStatus(string text)
        {
            throw new NotImplementedException();
        }

        public Task<Tweet> Retweet(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Tweet> Reply(long id, string text)
        {
            throw new NotImplementedException();
        }
    }
}