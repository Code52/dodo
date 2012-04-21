using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Modules.Streaming;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{
    public static class UserStreamExtensions
    {
        public static async Task<IUserStream> GetUserStream(this IUserSession session)
        {
            Func<Task<HttpResponseMessage>> startConnection =
                () =>
                {
                    var parameters = new SortedDictionary<string, string>();
                    var request = session.CreateGet(@"user.json", parameters);
                    var c = new HttpClient();
                    return c.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                };

            return new UserStream(startConnection);
        }
    }
}
