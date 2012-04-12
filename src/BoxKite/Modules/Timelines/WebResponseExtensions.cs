using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace BoxKite.Modules
{
    public static class WebResponseExtensions
    {
        public static IEnumerable<T2> MapTo<T, T2>(this WebResponse response, Func<T, IEnumerable<T2>> callback)
        {
            var resp = (HttpWebResponse)response;
            var stream = resp.GetResponseStream();

            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var objects = JsonConvert.DeserializeObject<T>(content);

            return callback(objects);
        }

    }
}