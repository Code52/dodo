using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace BoxKite.Extensions
{
    public static class WebResponseExtensions
    {
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