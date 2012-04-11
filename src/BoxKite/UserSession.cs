namespace BoxKite
{
    public class UserSession : IUserSession
    {
        private readonly string _token;

        public UserSession(string token)
        {
            _token = token;
        }
    }
}