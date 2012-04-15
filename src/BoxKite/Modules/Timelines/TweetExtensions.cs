using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Extensions;
using BoxKite.Mappings;
using Tweet = BoxKite.Models.Internal.Tweet;

namespace BoxKite.Modules.Timelines
{
    // TODO: upload image overload
    // TODO: status with latitude/longitude
     
    public static class TweetExtensions
    {
        static readonly Func<Tweet, Models.Tweet> Callback = o => new Models.Tweet
        {
            Id = o.id_str,
            Text = o.text,
            User = o.user.MapUser(),
            Time = o.created_at.ParseDateTime()
        };

        public async static Task<Models.Tweet> Tweet(this IUserSession session, string text)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"status", text},
                                 };
            var req = session.PostAsync("statuses/update.json", parameters);
            var response = await req;
            return response.MapTo(Callback);
        }

        public async static Task<Models.Tweet> Reply(this IUserSession session, Models.Tweet tweet, string text)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"status", text},
                                     {"in_reply_to_status_id", tweet.Id}
                                 };
            var req = session.PostAsync("statuses/update.json", parameters);
            var response = await req;
            return response.MapTo(Callback);
        }

        public async static Task<Models.Tweet> Retweet(this IUserSession session, Models.Tweet tweet)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     
                                 };
            var path = string.Format("statuses/retweet/{0}.json", tweet.Id);

            var req = session.PostAsync(path, parameters);
            var response = await req;
            return response.MapTo(Callback);
        }
    }
}
