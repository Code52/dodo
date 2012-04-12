using System;
using System.Collections.ObjectModel;
using BoxKite;
using BoxKite.Models;
using BoxKite.Modules;
using Dodo.Logic.Shared;
using Windows.UI.Core;
using BindableBase = Dodo.Logic.Shared.BindableBase;

namespace Dodo.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase
    {
        private readonly ITwitterService _twitter;
        private IUserSession _session;
        private readonly CoreDispatcher _dispatcher;

        public DashboardViewModel(ITwitterService twitter, CoreDispatcher dispatcher)
        {
            _twitter = twitter;
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
            _tasks.Add(new UserTask { Title = "Sign In", Command = new DelegateCommand(StartOAuthFlow) });

            _twitter.GetSession()
                    .SearchFor("twitter", pages: 5)
                    .Subscribe(OnNext);
        }

        private async void StartOAuthFlow()
        {
            // these creds are from a sample app that @shiftkey owns - you shouldn't trust them :)
            const string client = "pdFvR4kdbQgugKVIQ205Cw";
            const string secret = "J9xJaIBBfH2by22bRATVC6HfVf1WGermItci7Cx0Yw";

            _credentials = await TwitterAuthenticator.AuthenticateUser(client, "http://code52.org/twitter", secret);

            if (!_credentials.Valid)
                return;

            // TODO: save credentials to store

            _session = _twitter.GetUserSession(_credentials);

            _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, SetupApplication, this, null);
        }

        private TwitterCredentials _credentials;

        private void SetupApplication(object sender, InvokedHandlerArgs e)
        {
            Tweets.Clear();

            Tasks.Clear();
            Tasks.Add(new UserTask { Title = "Mentions", Command = new DelegateCommand(GetMentions) });
            Tasks.Add(new UserTask { Title = "Retweets" });
            Tasks.Add(new UserTask { Title = "New Followers" });
            Tasks.Add(new UserTask { Title = "Lost Followers" });
        }

        private void GetMentions()
        {
            Tweets.Clear();
            _session.GetMentions()
                    .Subscribe(OnNext);
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
    }
}
