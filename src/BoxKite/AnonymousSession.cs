using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Models;
using BoxKite.Modules;

namespace BoxKite
{
    public class AnonymousSession : IAnonymousSession
    {
        public IObservable<Tweet> SearchFor(string phrase, int resultsPerPage = 50, int pages = 1)
        {
            var listOfRequests = new List<WebRequest>();

            for (var i = 1; i <= pages; i++)
            {
                listOfRequests.Add(WebRequest.Create(new Uri(string.Format("http://search.twitter.com/search.json?q={0}&rpp={1}&page={2}", phrase, resultsPerPage, i))));
            }

            return listOfRequests.ToObservable()
                                 .SelectMany(req => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                                 .SelectMany(a => a.MapToTweets());
        }
    }
}