namespace BoxKite
{
    public interface ITwitterService
    {
        IUserSession GetUserSession(string token);
        IAnonymousSession GetSession();
    }
}
