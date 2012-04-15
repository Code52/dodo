using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using BoxKite.Mappings;
using BoxKite.Models;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{
    public static class TimelineExtensions
    {
        public static IObservable<Tweet> GetMentions(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "200"},
                                     {"include_entities", "true"},
                                     {"include_rts", "true"}
                                 };
            return session.AuthenticatedGet("statuses/mentions.json", parameters)
                             .ToObservable()
                             .Select(c => c.Content.ReadAsStringAsync().Result)
                             .SelectMany(r => r.FromTweet());
        }

        public static IObservable<Tweet> GetHomeTimeline(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "200"},
                                     {"include_entities", "true"},
                                     {"include_rts", "true"}
                                 };
            return session.AuthenticatedGet("statuses/home_timeline.json", parameters)
                          .ToObservable()
                          .Select(c => c.Content.ReadAsStringAsync().Result)
                          .SelectMany(r => r.FromTweet());
        }

        public static IObservable<Tweet> GetRetweets(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", "100"},
                                     {"include_entities", "true"},
                                 };
            return session.AuthenticatedGet("statuses/retweeted_to_me.json", parameters)
                          .ToObservable()
                          .Select(c => c.Content.ReadAsStringAsync().Result)
                          .SelectMany(r => r.FromTweet());
        }
    }
}
