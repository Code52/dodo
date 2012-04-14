using System;
using BoxKite.Models;

namespace BoxKite
{
    [Obsolete("Useless class")]
    public class TwitterService : ITwitterService
    {
        // TODO: userstream support

        public IUserSession GetUserSession(TwitterCredentials credentials)
        {
            return new UserSession(credentials);
        }

        public IAnonymousSession GetSession()
        {
            return new AnonymousSession();
        }
    }
}