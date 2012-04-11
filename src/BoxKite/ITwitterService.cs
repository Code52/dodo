using BoxKite.Models;

namespace BoxKite
{
    public interface ITwitterService
    {
        IUserSession GetUserSession(TwitterCredentials credentials);
        IAnonymousSession GetSession();
    }
}
