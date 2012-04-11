using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Models;
using Newtonsoft.Json;

namespace BoxKite
{
    public class AnonymousSession : IAnonymousSession
    {
        public IObservable<Tweet> SearchFor(string phrase, int resultsPerPage = 50, int pages = 1)
        {
            // TODO: support for multiple pages
            var request = WebRequest.Create(new Uri(string.Format("http://search.twitter.com/search.json?q={0}&rpp={1}", phrase, resultsPerPage)));

            var task = Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            return Observable.FromAsync(() => task).SelectMany(MapToTweets);
        }

        private static IEnumerable<Tweet> MapToTweets(WebResponse response)
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