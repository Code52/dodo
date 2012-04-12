using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BoxKite.Models;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BoxKite
{
    public class UserSession : IUserSession
    {
        readonly TwitterCredentials _credentials;

        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        public UserSession(TwitterCredentials credentials)
        {
            _credentials = credentials;
        }

        public Task<Tweet> UpdateStatus(string text)
        {
            throw new NotImplementedException();
        }

        public Task<Tweet> Retweet(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Tweet> Reply(long id, string text)
        {
            throw new NotImplementedException();
        }

        public WebRequest AuthenticatedGet(string relativeUrl, SortedDictionary<string, string> parameters)
        {
            var url = "https://api.twitter.com/1/" + relativeUrl;

            var oauthToken = _credentials.Token;
            var oauthConsumerKey = _credentials.ConsumerKey;
            var rand = new Random();
            var oauthNonce = rand.Next(1000000000).ToString();

            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var oauthTimestamp = Convert.ToInt64(ts.TotalSeconds).ToString();

            //GS - When building the signature string the params
            //must be in alphabetical order. I can't be bothered
            //with that, get SortedDictionary to do it's thing
            var sd = new SortedDictionary<string, string>
                         {
                             {"oauth_consumer_key", oauthConsumerKey},
                             {"oauth_nonce", oauthNonce},
                             {"oauth_signature_method", OauthSignatureMethod},
                             {"oauth_timestamp", oauthTimestamp},
                             {"oauth_token", oauthToken},
                             {"oauth_version", OauthVersion}
                         };

            var querystring = "";

            var baseString = "GET&" + Uri.EscapeDataString(url) + "&";
            foreach (var entry in parameters)
            {
                querystring += entry.Key + "=" + entry.Value + "&";
                baseString += Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&");
            }

            foreach (var entry in sd)
            {
                baseString += Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&");
            }

            //GS - Remove the trailing ambersand char, remember 
            //it's been urlEncoded so you have to remove the 
            //last 3 chars - %26
            baseString = baseString.Substring(0, baseString.Length - 3);

            var signingKey = Uri.EscapeDataString(_credentials.ConsumerSecret) + "&" + Uri.EscapeDataString(_credentials.TokenSecret);

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(signingKey, BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(baseString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signatureString = CryptographicBuffer.EncodeToBase64String(signatureBuffer);

            var fullUrl = url;
            if (!string.IsNullOrWhiteSpace(querystring))
            {
                fullUrl += "?" + querystring.Substring(0, querystring.Length - 1);
            }

            var hwr = (HttpWebRequest)WebRequest.Create(fullUrl);

            hwr.Headers["Authorization"] = string.Format(
                "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", oauth_token=\"{4}\", oauth_signature=\"{5}\", oauth_version=\"{6}\"",
                Uri.EscapeDataString(oauthNonce),
                Uri.EscapeDataString(OauthSignatureMethod),
                Uri.EscapeDataString(oauthTimestamp),
                Uri.EscapeDataString(oauthConsumerKey),
                Uri.EscapeDataString(oauthToken),
                Uri.EscapeDataString(signatureString),
                Uri.EscapeDataString(OauthVersion));

            return hwr;
        }
    }
}