using System;
using System.Threading.Tasks;
using BoxKite.Models;

namespace BoxKite
{
    public interface IUserSession
    {
        IObservable<Tweet> GetTimeline();
        IObservable<Tweet> GetMentions();
        IObservable<Tweet> GetDirectMessages();
        IObservable<Tweet> GetRetweets();

        // TODO: upload image overload
        // TODO: status with latitude/longitude
        Task<Tweet> UpdateStatus(string text); 
        Task<Tweet> Retweet(long id);
        Task<Tweet> Reply(long id, string text);
    }
}
