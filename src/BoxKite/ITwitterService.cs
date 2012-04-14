using System;
using BoxKite.Models;

namespace BoxKite
{
    [Obsolete("Useless class")]
    public interface ITwitterService
    {
        IUserSession GetUserSession(TwitterCredentials credentials);
        IAnonymousSession GetSession();
    }
}
