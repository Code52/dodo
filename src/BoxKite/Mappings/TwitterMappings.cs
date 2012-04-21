using System.Collections.Generic;
using System.Linq;
using BoxKite.Extensions;
using BoxKite.Models.Internal;
using Newtonsoft.Json;
using Tweet = BoxKite.Models.Internal.Tweet;
using User = BoxKite.Models.User;

namespace BoxKite.Mappings
{
    public static class TwitterMappings
    {
        public static IEnumerable<Models.Tweet> FromResponse(this string body)
        {
            var objects = JsonConvert.DeserializeObject<List<Tweet>>(body);

            return objects.Select(o => new Models.Tweet
            {
                Id = o.id_str,
                Text = o.text,
                User = MapUser(o.user),
                Time = o.created_at.ToDateTimeOffset()
            });
        }

        public static Models.Tweet FromTweet(this string body)
        {
            var o = JsonConvert.DeserializeObject<Tweet>(body);

            if (o.retweeted_status != null)
            {
                var status = o.retweeted_status;
                return new Models.Tweet
                {
                    Id = status.id_str,
                    Text = status.text,
                    User = MapUser(status.user),
                    RetweetedBy = MapUser(o.user),
                    Time = status.created_at.ToDateTimeOffset()
                };
            }

            return new Models.Tweet
                       {
                           Id = o.id_str,
                           Text = o.text,
                           User = MapUser(o.user),
                           Time = o.created_at.ToDateTimeOffset()
                       };
        }

        public static IEnumerable<Models.Tweet> FromSearchResponse(this string body)
        {
            var result = JsonConvert.DeserializeObject<SearchResponse>(body);

            return result.results.Select(c => new Models.Tweet
            {
                Id = c.id_str,
                Text = c.text,
                User = MapUser(c),
                Time = c.created_at.ToDateTimeOffset()
            });
        }

        internal static User MapUser(this Models.Internal.User user)
        {
            return new User
            {
                Name = user.screen_name,
                Avatar = user.profile_image_url_https
            };
        }

        private static User MapUser(Result result)
        {
            return new User
            {
                Name = result.from_user,
                Avatar = result.profile_image_url_https
            };
        }
    }
}

