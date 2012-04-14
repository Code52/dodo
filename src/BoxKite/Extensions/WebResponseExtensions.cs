using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace BoxKite.Extensions
{
    public static class WebResponseExtensions
    {
        [Obsolete("HttpWebRequest is evil. Eeeeviiiiil")]
        public static IEnumerable<TOutput> MapTo<TResponse, TOutput>(this WebResponse response, Func<TResponse, IEnumerable<TOutput>> callback)
        {
            var resp = (HttpWebResponse)response;
            var stream = resp.GetResponseStream();

            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            var objects = JsonConvert.DeserializeObject<TResponse>(content);

            return callback(objects);
        }

        public static IEnumerable<TOutput> MapTo<TResponse, TOutput>(this HttpResponseMessage response, Func<TResponse, IEnumerable<TOutput>> callback)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            var objects = JsonConvert.DeserializeObject<TResponse>(content);
            return callback(objects);
        }

        public static TOutput MapTo<TResponse, TOutput>(this HttpResponseMessage response, Func<TResponse, TOutput> callback)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            var objects = JsonConvert.DeserializeObject<TResponse>(content);
            return callback(objects);
        }
    }
}