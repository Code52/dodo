using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Models;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{
    public static class TimelineExtensions
    {
        public static IObservable<Tweet> GetMentions(this IUserSession session)
        {
            var req = session.AuthenticatedGet("statuses/mentions.json?count=200&include_rts=1&include_entities=true");
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapToTweets());
        }

        public static IObservable<Tweet> GetHomeTimeline(this IUserSession session)
        {
            var req = session.AuthenticatedGet("statuses/home_timeline.json?count=200&include_rts=1&include_entities=true");
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapToTweets());
        }
    }
}
