using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BoxKite.Models;
using Newtonsoft.Json;

namespace BoxKite.Modules
{
    public static class WebResponseExtensions
    {
        public static IEnumerable<Tweet> MapToTweets(this WebResponse response)
        {
            var resp = (HttpWebResponse)response;
            var stream = resp.GetResponseStream();

            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var objects = JsonConvert.DeserializeObject<SearchResponse>(content);

            return objects.results.Select(c => new Tweet { Text = c.text, Author = c.from_user, Avatar = c.profile_image_url_https });
        }

    }
}