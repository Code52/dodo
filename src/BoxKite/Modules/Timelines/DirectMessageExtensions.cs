using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using BoxKite.Extensions;
using BoxKite.Mappings;
using BoxKite.Models;
using BoxKite.Models.Internal;

// ReSharper disable CheckNamespace
namespace BoxKite.Modules
// ReSharper restore CheckNamespace
{ 
    public static class DirectMessageExtensions
    {
        static readonly Func<List<DM>, IEnumerable<DirectMessage>> Callback = c => c.Select(o => new DirectMessage
        {
            Text = o.text,
            User = o.sender.MapUser(),
            Recipient = o.recipient.name,
            Time = o.created_at.ParseDateTime()
        }).OrderByDescending(o => o.Time);

        public static IObservable<DirectMessage> GetDirectMessages(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };
            var req = session.GetAsync("direct_messages.json", parameters);
            return req.ToObservable().SelectMany(a => a.MapTo(Callback));
        }

        public static IObservable<DirectMessage> GetSentDirectMessages(this IUserSession session)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"include_entities", "true"},
                                 };
            var req = session.GetAsync("direct_messages/sent.json", parameters);
            return req.ToObservable().SelectMany(a => a.MapTo(Callback));
        }
    }
}