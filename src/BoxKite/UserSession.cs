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

        public WebRequest AuthenticatedGet(string relativeUrl)
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
                             {"oauth_version", OauthVersion},
                             {"oauth_consumer_key", oauthConsumerKey},
                             {"oauth_nonce", oauthNonce},
                             {"oauth_signature_method", OauthSignatureMethod},
                             {"oauth_timestamp", oauthTimestamp},
                             {"oauth_token", oauthToken}
                         };

            var baseString = "GET&" + Uri.EscapeDataString(url) + "&";
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

            var hwr = (HttpWebRequest)WebRequest.Create(url);

            var authorizationHeaderParams = "OAuth ";
            authorizationHeaderParams += "oauth_nonce=" + "\"" + Uri.EscapeDataString(oauthNonce) + "\",";
            authorizationHeaderParams += "oauth_signature_method=" + "\"" + Uri.EscapeDataString(OauthSignatureMethod) + "\",";
            authorizationHeaderParams += "oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauthTimestamp) + "\",";
            authorizationHeaderParams += "oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauthConsumerKey) + "\",";
            authorizationHeaderParams += "oauth_token=" + "\"" + Uri.EscapeDataString(oauthToken) + "\",";
            authorizationHeaderParams += "oauth_signature=" + "\"" + Uri.EscapeDataString(signatureString) + "\",";
            authorizationHeaderParams += "oauth_version=" + "\"" + Uri.EscapeDataString(OauthVersion) + "\"";

            hwr.Headers["Authorization"] = authorizationHeaderParams;

            return hwr;
        }
    }
}