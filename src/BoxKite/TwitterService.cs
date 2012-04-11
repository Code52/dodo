using BoxKite.Models;

namespace BoxKite
{
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