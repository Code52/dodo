using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxKite
{
    public interface IUserSession
    {
        // TODO: upload image overload
        // TODO: status with latitude/longitude
        Task<HttpResponseMessage> AuthenticatedGet(string relativeUrl, SortedDictionary<string, string> parameters);
        Task<HttpResponseMessage> AuthenticatedPost(string relativeUrl, SortedDictionary<string, string> parameters);
    }
}
