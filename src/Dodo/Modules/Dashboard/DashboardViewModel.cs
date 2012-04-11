using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using BoxKite;
using BoxKite.Models;
using BoxKite.Modules;
using Dodo.Logic.Shared;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.UI.Core;
using BindableBase = Dodo.Logic.Shared.BindableBase;

namespace Dodo.Modules.Dashboard
{
    public class DashboardViewModel : BindableBase
    {
        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        private readonly ITwitterService _twitter;
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

            var result = await TwitterAuthenticator.AuthenticateUser(client, "http://code52.org/twitter", secret);

            if (result.ResponseStatus != WebAuthenticationStatus.Success)
                return;

            _credentials = await TwitterAuthenticator.GetUserCredentials(client, secret, result.ResponseData);

            if (_credentials == null)
                return;

            // TODO: spotting OAuth exception - remove this call after
            var request = _twitter.GetUserSession(_credentials)
                                  .AuthenticatedGet("statuses/mentions.json?count=200&include_rts=1&include_entities=true");

            var response = await request.GetResponseAsync();

            if (_credentials != null)
            {
                _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, SetupApplication, this, null);
            }
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
            _twitter.GetUserSession(_credentials)
                                  .GetMentions()
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
