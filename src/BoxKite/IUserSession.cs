using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxKite
{
    public interface IUserSession
    {
        Task<HttpResponseMessage> GetAsync(string relativeUrl, SortedDictionary<string, string> parameters);
        Task<HttpResponseMessage> PostAsync(string relativeUrl, SortedDictionary<string, string> parameters);
        HttpRequestMessage CreateGet(string fullUrl, SortedDictionary<string, string> parameters);
    }
}
