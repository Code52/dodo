using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Mappings;
using BoxKite.Models;
using Newtonsoft.Json;

namespace BoxKite.Modules.Streaming
{
    public class UserStream : IUserStream
    {
        readonly Stream _stream;
        readonly Subject<Tweet> _tweets = new Subject<Tweet>();
        readonly Subject<long> _friends = new Subject<long>();
        bool _isActive = true;

        public UserStream(Stream dataSource)
        {
            _stream = dataSource;
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessMessages);
        }

        public void Stop()
        {
            _isActive = false;
        }

        public IObservable<Tweet> Tweets { get { return _tweets; } }

        public IObservable<long> Friends { get { return _friends; } }

        private void ProcessMessages()
        {
            var responseStream = new StreamReader(_stream);
            while (_isActive)
            {
                var line = responseStream.ReadLine();
                if (String.IsNullOrEmpty(line)) continue;

                var obj = JsonConvert.DeserializeObject<dynamic>(line);

                if (obj["friends"] != null)
                {
                    SendFriendsMessage(obj);
                    continue;
                }

                if (obj["event"] != null)
                {
                    // TODO: process event
                    continue;
                }

                var tweet = line.FromTweet();
                if (tweet != null)
                {
                    _tweets.OnNext(tweet);
                }
            }
        }

        private void SendFriendsMessage(dynamic obj)
        {
            foreach (var friend in obj.friends)
            {
                long temp;
                if (long.TryParse(friend.ToString(), out temp))
                    _friends.OnNext(temp);
            }
        }

        public void Dispose()
        {
            _isActive = false;

            _friends.Dispose();
            _tweets.Dispose();
        }
    }
}