using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Mappings;
using BoxKite.Models;

namespace BoxKite
{
    public class AnonymousSession : IAnonymousSession
    {
        public IObservable<Tweet> SearchFor(string phrase, int resultsPerPage = 50, int pages = 1)
        {
            var client = new HttpClient();
            var listOfRequests = new List<Task<HttpResponseMessage>>();

            for (var i = 1; i <= pages; i++)
            {
                var task = client.GetAsync(new Uri(string.Format("http://search.twitter.com/search.json?q={0}&rpp={1}&page={2}", phrase, resultsPerPage, i)));
                listOfRequests.Add(task);
            }

            return listOfRequests.ToObservable()
                                 .Select(c => c.Result.Content.ReadAsStringAsync().Result)
                                 .SelectMany(a => a.FromSearchResponse());
        }
    }
}