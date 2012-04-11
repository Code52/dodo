namespace BoxKite
{
    public class TwitterService : ITwitterService
    {
        // TODO: userstream support

        public IUserSession GetUserSession(string token)
        {
            return new UserSession(token);
        }

        public IAnonymousSession GetSession()
        {
            return new AnonymousSession();
        }
    }
}