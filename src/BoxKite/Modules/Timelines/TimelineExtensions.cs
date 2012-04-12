using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Func<List<Mention>, IEnumerable<Tweet>> callback = c => c.Select(o => new Tweet
                                                                                                 {
                                                                                                     Text = o.text, 
                                                                                                     Author = o.user.name, 
                                                                                                     Avatar = o.user.profile_image_url_https
                                                                                                 });

        public static IObservable<Tweet> GetMentions(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "200"},
                                     {"include_entities", "true"},
                                     {"include_rts", "true"}
                                 };
            var req = session.AuthenticatedGet("statuses/mentions.json", parameters);
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapTo(callback));
        }

        public static IObservable<Tweet> GetHomeTimeline(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "200"},
                                     {"include_entities", "true"},
                                     {"include_rts", "true"}
                                 };
            var req = session.AuthenticatedGet("statuses/home_timeline.json", parameters);
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapTo(callback));
        }
    }
}
