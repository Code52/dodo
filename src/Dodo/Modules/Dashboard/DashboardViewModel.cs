using System;
using System.Collections.ObjectModel;
using BoxKite;
using BoxKite.Authentication;
using BoxKite.Models;
using BoxKite.Modules;
using Dodo.Logic;
using Dodo.Logic.Extensions;
using Dodo.Logic.Shared;
using Windows.UI.Core;

namespace Dodo.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase
    {
        readonly Func<IAnonymousSession> _getAnonymousSession;
        readonly Func<TwitterCredentials, IUserSession> _getUserSession;
        readonly IDiagnosticService _diagnostics;
        readonly ISettingsService _settings;
        readonly CoreDispatcher _dispatcher;

        public DashboardViewModel(
            Func<IAnonymousSession> getAnonymousSession,
            Func<TwitterCredentials,IUserSession> getUserSession,
            IDiagnosticService diagnostics,
            ISettingsService settings,
            CoreDispatcher dispatcher)
        {
            _getAnonymousSession = getAnonymousSession;
            _getUserSession = getUserSession;
            _diagnostics = diagnostics;
            _settings = settings;
            _dispatcher = dispatcher;
        }

        private readonly ObservableCollection<UserTask> _tasks = new ObservableCollection<UserTask>();
        public ObservableCollection<UserTask> Tasks
        {
            get { return _tasks; }
        }

        private readonly ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();
        public ObservableCollection<Tweet> Tweets
        {
            get { return _tweets; }
        }

        public void Start()
        {
            if (_settings.HasUserSettings())
            {
                _credentials = _settings.GetUserCredentials();
                _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, SetupApplication, this, null);
            }
            else
            {
                _tasks.Add(new UserTask { Title = "Sign In", Command = new DelegateCommand(StartOAuthFlow) });

                _getAnonymousSession()
                        .SearchFor("twitter", pages: 5)
                        .Subscribe(OnNext);                
            }
        }

        private async void StartOAuthFlow()
        {
            // these creds are from a sample app that @shiftkey owns - you shouldn't trust them :)
            const string client = "pdFvR4kdbQgugKVIQ205Cw";
            const string secret = "J9xJaIBBfH2by22bRATVC6HfVf1WGermItci7Cx0Yw";

            _credentials = await TwitterAuthenticator.AuthenticateUser(client, "http://code52.org/twitter", secret);

            if (!_credentials.Valid)
                return;

            _settings.StoreUserCredentials(_credentials);

            _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, SetupApplication, this, null);
        }

        private TwitterCredentials _credentials;

        private void SetupApplication(object sender, InvokedHandlerArgs e)
        {
            Tweets.Clear();
            Tasks.Clear();

            Tasks.Add(new UserTask { Title = "Timeline", Command = new DelegateCommand(GetTimeline) });
            Tasks.Add(new UserTask { Title = "Mentions", Command = new DelegateCommand(GetMentions) });
            Tasks.Add(new UserTask { Title = "Retweets", Command = new DelegateCommand(GetRetweets) });
            Tasks.Add(new UserTask { Title = "DMs", Command = new DelegateCommand(GetDirectMessages) });
            Tasks.Add(new UserTask { Title = "New Followers" });
            Tasks.Add(new UserTask { Title = "Lost Followers" });

            StartStreaming();
        }

        private async void StartStreaming()
        {
            Tweets.Clear();

            var session = _getUserSession(_credentials);

            session
                .GetHomeTimeline()
                .Subscribe(OnNext, OnError);
            
            var stream = await session.GetUserStream();
            stream.Tweets.Subscribe(OnStreamed, OnError);
            stream.Start();
        }

        private void GetTimeline()
        {
            Tweets.Clear();
            _getUserSession(_credentials)
                .GetHomeTimeline()
                .Subscribe(OnNext, OnError);
        }

        private void GetRetweets()
        {
            Tweets.Clear();
            _getUserSession(_credentials)
                .GetRetweets()
                .Subscribe(OnNext, OnError);
        }

        private void GetMentions()
        {
            Tweets.Clear();
            _getUserSession(_credentials)
                .GetMentions()
                .Subscribe(OnNext, OnError);
        }

        private void GetDirectMessages()
        {
            Tweets.Clear();

            _getUserSession(_credentials)
                .GetDirectMessages()
                .Subscribe(OnNext, OnError);

            _getUserSession(_credentials)
                .GetSentDirectMessages()
                .Subscribe(OnNext, OnError);
        }

        private void OnError(Exception ex)
        {
            _diagnostics.LogError("Dashboard", ex);
        }
        
        private void OnNext(Tweet tweet)
        {
            _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, AddTweet, this, tweet);
        }

        private void AddTweet(object sender, InvokedHandlerArgs e)
        {
            var tweet = e.Context as Tweet;
            Tweets.Add(tweet);
        }

        private void OnStreamed(Tweet tweet)
        {
            _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, AddTweetFirst, this, tweet);
        }

        private void AddTweetFirst(object sender, InvokedHandlerArgs e)
        {
            var tweet = e.Context as Tweet;
            Tweets.Insert(0, tweet);
        }
    }
}