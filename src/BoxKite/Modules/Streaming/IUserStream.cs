using System;
using BoxKite.Models;

namespace BoxKite.Modules.Streaming
{
    // TODO: expose stream of events
    
    public interface IUserStream : IDisposable
    {
        IObservable<Tweet> Tweets { get; }
        IObservable<long> Friends { get; }
        void Start();
        void Stop();
    }
}