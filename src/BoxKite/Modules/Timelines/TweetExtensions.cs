using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxKite.Extensions;
using BoxKite.Models;

namespace BoxKite.Modules.Timelines
{
    public static class TweetExtensions
    {
        static readonly Func<Mention, Tweet> Callback = o => new Tweet
        {
            Id = o.id_str,
            Text = o.text,
            Author = o.user.name,
            Avatar = o.user.profile_image_url_https,
            Time = o.created_at.ParseDateTime()
        };

        public async static Task<Tweet> Tweet(this IUserSession session, string text)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"status", text},
                                 };
            var req = session.AuthenticatedPost("statuses/update.json", parameters);
            var response = await req;
            return response.MapTo(Callback);
        }

        public async static Task<Tweet> Reply(this IUserSession session, Tweet tweet, string text)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"status", text},
                                     {"in_reply_to_status_id", tweet.Id}
                                 };
            var req = session.AuthenticatedPost("statuses/update.json", parameters);
            var response = await req;
            return response.MapTo(Callback);
        }

        public async static Task<Tweet> Retweet(this IUserSession session, Tweet tweet)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     
                                 };
            var path = string.Format("statuses/retweet/{0}.json", tweet.Id);

            var req = session.AuthenticatedPost(path, parameters);
            var response = await req;
            return response.MapTo(Callback);
        }
    }
}
