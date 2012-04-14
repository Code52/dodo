using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace BoxKite.Modules
{
    public static class WebResponseExtensions
    {
        public static IEnumerable<TOutput> MapTo<TResponse, TOutput>(this WebResponse response, Func<TResponse, IEnumerable<TOutput>> callback)
        {
            var resp = (HttpWebResponse)response;
            var stream = resp.GetResponseStream();

            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var objects = JsonConvert.DeserializeObject<TResponse>(content);

            return callback(objects);
        }

        public static TOutput MapTo<TResponse, TOutput>(this WebResponse response, Func<TResponse, TOutput> callback)
        {
            var resp = (HttpWebResponse)response;
            var stream = resp.GetResponseStream();

            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var objects = JsonConvert.DeserializeObject<TResponse>(content);

            return callback(objects);
        }

    }
}