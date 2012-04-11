using System;
using BoxKite.Models;

namespace BoxKite
{
    public interface IAnonymousSession
    {
        IObservable<Tweet> SearchFor(string phrase, int resultsPerPage = 50, int pages = 1);
    }
}