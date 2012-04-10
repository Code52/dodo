using System;
using Dodo.Logic.Shared;

namespace Dodo.Logic.Services
{
    public interface ITwitterService
    {
        IObservable<Tweet> GetSampleTweets();
    }
}
