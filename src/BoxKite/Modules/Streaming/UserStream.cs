using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BoxKite.Mappings;
using BoxKite.Models;
using Newtonsoft.Json;

namespace BoxKite.Modules.Streaming
{
    public class UserStream : IUserStream
    {
        readonly Func<Task<HttpResponseMessage>> _createOpenConnection;
        readonly Subject<Tweet> _tweets = new Subject<Tweet>();
        readonly Subject<long> _friends = new Subject<long>();
        readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(20);

        bool _isActive = true;
        TimeSpan _delay = TimeSpan.FromSeconds(20);

        public UserStream(Func<Task<HttpResponseMessage>> createOpenConnection)
        {
            _createOpenConnection = createOpenConnection;
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessMessages)
                .ContinueWith(HandleExceptionsIfRaised);
        }

        private void HandleExceptionsIfRaised(Task obj)
        {
            if (obj.Exception != null)
            {
                SendToAllSubscribers(obj.Exception);
            }

            if (obj.IsFaulted)
            {
                SendToAllSubscribers(new Exception("Stream is faulted"));
            }

            if (obj.IsCanceled)
            {
                SendToAllSubscribers(new Exception("Stream is cancelled"));
            }
        }

        private void SendToAllSubscribers(Exception exception)
        {
            _tweets.OnError(exception);
            _friends.OnError(exception);
        }

        public void Stop()
        {
            _isActive = false;
        }

        public IObservable<Tweet> Tweets { get { return _tweets; } }

        public IObservable<long> Friends { get { return _friends; } }

        private async void ProcessMessages()
        {
            var responseStream = await GetStream();
            while (_isActive)
            {
                // reconnect if the stream was closed previously
                if (responseStream == null)
                {
                    await Task.Delay(_delay);
                    responseStream = await GetStream();
                }

                string line;
                try
                {
                    line = responseStream.ReadLine();
                }
                catch (IOException)
                {
                    _delay += InitialDelay;
                    responseStream.Dispose();
                    responseStream = null;
                    line = "";
                }

                if (_delay.TotalMinutes <= 2)
                {
                    // TODO: give up
                }

                if (String.IsNullOrEmpty(line)) continue;

                Debug.WriteLine(line);

                // we have a valid connection - clear delay
                _delay = TimeSpan.Zero;

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

        private async Task<StreamReader> GetStream()
        {
            var response = await _createOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();

            var responseStream = new StreamReader(stream);
            return responseStream;
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