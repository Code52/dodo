using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Models;
using Dodo.Logic.Shared;
using Newtonsoft.Json;

namespace Dodo.Logic.Services
{
    public class TwitterService : ITwitterService
    {
        public IObservable<Tweet> GetSampleTweets()
        {
            var request = HttpWebRequest.Create(new Uri("http://search.twitter.com/search.json?q=twitter"));

            var task = Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            return Observable.FromAsync(() => task).SelectMany(MapToTweets);
        }

        protected IEnumerable<Tweet> MapToTweets(WebResponse response)
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