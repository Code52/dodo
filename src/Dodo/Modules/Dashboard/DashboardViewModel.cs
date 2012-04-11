using System;
using System.Collections.ObjectModel;
using BoxKite;
using Dodo.Logic.Services;
using Dodo.Logic.Shared;
using Windows.Security.Authentication.Web;
using Windows.UI.Core;

namespace Dodo
{
    public class DashboardViewModel : BindableBase
    {
        private readonly ITwitterService _twitter;
        private readonly CoreDispatcher _dispatcher;

        public DashboardViewModel(ITwitterService twitter, CoreDispatcher dispatcher)
        {
            _twitter = twitter;
            _dispatcher = dispatcher;
        }

        private ObservableCollection<UserTask> _tasks = new ObservableCollection<UserTask>();
        public ObservableCollection<UserTask> Tasks
        {
            get { return _tasks; }
        }

        private ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();
        public ObservableCollection<Tweet> Tweets
        {
            get { return _tweets; }
        }

        public void Start()
        {
            _tasks.Add(new UserTask { Title = "Sign In", Command = new DelegateCommand(StartOAuthFlow) });

            _twitter.GetSampleTweets().Subscribe(OnNext);
        }

        private async void StartOAuthFlow()
        {
            // these creds are from a sample app that @shiftkey owns - you shouldn't trust them :)
            var result = await TwitterAuthenticator.AuthenticateUser("pdFvR4kdbQgugKVIQ205Cw", "http://code52.org/twitter", "J9xJaIBBfH2by22bRATVC6HfVf1WGermItci7Cx0Yw");

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, SetupApplication, this, result.ResponseData);
            }

            // TODO: error handling
        }

        private void SetupApplication(object sender, InvokedHandlerArgs e)
        {
            Tasks.Clear();
            Tasks.Add(new UserTask { Title = "Mentions" });
            Tasks.Add(new UserTask { Title = "Retweets" });
            Tasks.Add(new UserTask { Title = "New Followers" });
            Tasks.Add(new UserTask { Title = "Lost Followers" });
            // fire tasks in background

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
