using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BoxKite.Models;

namespace BoxKite
{
    public interface IUserSession
    {
        // TODO: upload image overload
        // TODO: status with latitude/longitude
        Task<Tweet> UpdateStatus(string text); 
        Task<Tweet> Retweet(long id);
        Task<Tweet> Reply(long id, string text);

        WebRequest AuthenticatedGet(string relativeUrl, SortedDictionary<string, string> parameters);
    }
}
