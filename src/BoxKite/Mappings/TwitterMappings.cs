using System.Collections.Generic;
using System.Linq;
using BoxKite.Extensions;
using BoxKite.Models;
using Newtonsoft.Json;

namespace BoxKite.Mappings
{
    public static class TwitterMappings
    {
        public static IEnumerable<Tweet> FromTweet(this string body)
        {
            var objects = JsonConvert.DeserializeObject<List<Mention>>(body);

            return objects.Select(o => new Tweet
                                           {
                                               Id = o.id_str,
                                               Text = o.text,
                                               Author = o.user.name,
                                               Avatar = o.user.profile_image_url_https,
                                               Time = o.created_at.ParseDateTime()
                                           }).OrderByDescending(o => o.Time);
        }


        public static IEnumerable<Tweet> FromSearchResponse(this string body)
        {
            var result = JsonConvert.DeserializeObject<SearchResponse>(body);

            return result.results.Select(c => new Tweet
            {
                Id = c.id_str,
                Text = c.text,
                Author = c.from_user,
                Avatar = c.profile_image_url_https,
            });

        }
    }
}

