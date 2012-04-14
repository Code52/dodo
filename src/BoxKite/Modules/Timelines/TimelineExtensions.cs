using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using BoxKite.Extensions;
using BoxKite.Models;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{
    public static class TimelineExtensions
    {
        static readonly Func<List<Mention>, IEnumerable<Tweet>> Callback = c => c.Select(o => new Tweet
                                                                                                 {
                                                                                                     Id = o.id_str,
                                                                                                     Text = o.text,
                                                                                                     Author = o.user.name,
                                                                                                     Avatar = o.user.profile_image_url_https,
                                                                                                     Time = o.created_at.ParseDateTime()
                                                                                                 }).OrderByDescending(o => o.Time);

        public static IObservable<Tweet> GetMentions(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "200"},
                                     {"include_entities", "true"},
                                     {"include_rts", "true"}
                                 };
            var req = session.AuthenticatedGet("statuses/mentions.json", parameters);
            return req.ToObservable().SelectMany(a => a.MapTo(Callback));
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
            return req.ToObservable().SelectMany(a => a.MapTo(Callback));
        }

        public static IObservable<Tweet> GetRetweets(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "100"},
                                     {"include_entities", "true"},
                                 };
            var req = session.AuthenticatedGet("statuses/retweeted_to_me.json", parameters);
            return req.ToObservable()
                      .SelectMany(a => a.MapTo(Callback));
        }
    }
}
