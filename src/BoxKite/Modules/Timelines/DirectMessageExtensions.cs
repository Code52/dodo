using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BoxKite.Models;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{
    public static class DirectMessageExtensions
    {
        private static Func<List<DM>, IEnumerable<DirectMessage>> callback = c => c.Select(o => new DirectMessage
        {
            Text = o.text,
            Author = o.sender.name,
            Avatar = o.sender.profile_image_url_https,
            Recipient = o.recipient.name
        });

        public static IObservable<DirectMessage> GetDirectMessages(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };
            var req = session.AuthenticatedGet("direct_messages.json", parameters);
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapTo(callback));
        }

        public static IObservable<DirectMessage> GetSentDirectMessages(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };
            var req = session.AuthenticatedGet("direct_messages/sent.json", parameters);
            return Observable.FromAsync(() => Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                             .SelectMany(a => a.MapTo(callback));
        }
    }
}
