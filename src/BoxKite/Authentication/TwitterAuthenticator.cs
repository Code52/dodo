using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BoxKite.Models;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BoxKite.Authentication
{
    public class TwitterAuthenticator
    {
        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        private static string UrlEncode(string toEncode)
        {
            var encoded = "";

            for (var i = 0; i < toEncode.Length; i++)
            {
                var test = toEncode[i];
                if ((test >= 'A' && test <= 'Z') ||
                    (test >= 'a' && test <= 'z') ||
                    (test >= '0' && test <= '9'))
                {
                    encoded += test;
                }
                else if (test == '-' || test == '_' || test == '.' || test == '~')
                {
                    encoded += test;
                }
                else
                {
                    encoded += "%" + string.Format("{0:X}", (int)test);
                }
            }
            return encoded;
        }

        private static async Task<string> PostData(string url, string data)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                request.Headers.Add("Authorization", data);
                var response = await client.SendAsync(request);
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<TwitterCredentials> AuthenticateUser(string twitterClientID, string twitterCallbackUrl, string twitterClientSecret)
        {
            if (string.IsNullOrWhiteSpace(twitterClientID))
                throw new ArgumentException("TwitterClientID must be specified", twitterClientID);

            if (string.IsNullOrWhiteSpace(twitterCallbackUrl))
                throw new ArgumentException("TwitterCallbackUrl must be specified", twitterCallbackUrl);

            if (string.IsNullOrWhiteSpace(twitterClientSecret))
                throw new ArgumentException("TwitterClientSecret must be specified", twitterClientSecret);

            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var sinceEpoch = Convert.ToInt64(ts.TotalSeconds).ToString();

            var rand = new Random();
            var twitterUrl = "https://api.twitter.com/oauth/request_token";
            var nonce = rand.Next(1000000000);

            var sigBaseStringParams =
                string.Format(
                    "oauth_callback={0}&oauth_consumer_key={1}&oauth_nonce={2}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={3}&oauth_version=1.0",
                    UrlEncode(twitterCallbackUrl),
                    twitterClientID,
                    nonce,
                    sinceEpoch);

            var sigBaseString = string.Format("POST&{0}&{1}", UrlEncode(twitterUrl), UrlEncode(sigBaseStringParams));

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(twitterClientSecret + "&", BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            var dataToPost = string.Format(
                    "OAuth oauth_callback=\"{0}\", oauth_consumer_key=\"{1}\", oauth_nonce=\"{2}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{3}\", oauth_version=\"1.0\", oauth_signature=\"{4}\"",
                    UrlEncode(twitterCallbackUrl),
                    twitterClientID,
                    nonce,
                    sinceEpoch,
                    UrlEncode(signature));

            var response = await PostData(twitterUrl, dataToPost);

            if (string.IsNullOrWhiteSpace(response))
                return TwitterCredentials.Null;

            string oauthToken = null;

            foreach (var splits in response.Split('&').Select(t => t.Split('=')))
            {
                switch (splits[0])
                {
                    case "oauth_token":
                        oauthToken = splits[1];
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(oauthToken))
                return TwitterCredentials.Null;

            twitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken;
            var startUri = new Uri(twitterUrl);
            var endUri = new Uri(twitterCallbackUrl);

            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);

            if (result.ResponseStatus != WebAuthenticationStatus.Success)
                return TwitterCredentials.Null;

            return await GetUserCredentials(twitterClientID, twitterClientSecret, result.ResponseData);
        }

        private static async Task<TwitterCredentials> GetUserCredentials(string consumerKey, string consumerSecret, string responseText)
        {
            var args = responseText.Substring(responseText.IndexOf("?") + 1)
                                   .Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            var verifier = "";
            var token = "";

            foreach (var a in args)
            {
                var index = a.IndexOf("=");
                var key = a.Substring(0, index);
                var value = a.Substring(index + 1);

                if (key.Equals("oauth_token", StringComparison.OrdinalIgnoreCase))
                    token = value;

                if (key.Equals("oauth_verifier", StringComparison.OrdinalIgnoreCase))
                    verifier = value;
            }

            var url = "https://api.twitter.com/oauth/access_token?oauth_verifier=" + verifier;

            var oauthToken = token;
            var oauthConsumerKey = consumerKey;
            var rand = new Random();
            var oauthNonce = rand.Next(1000000000).ToString();

            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var oauthTimestamp = Convert.ToInt64(ts.TotalSeconds).ToString();

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
            //it's been urlEncoded so you have to remove the last 3 chars - %26
            baseString = baseString.Substring(0, baseString.Length - 3);

            var signingKey = Uri.EscapeDataString(consumerSecret) + "&";

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(signingKey, BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(baseString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signatureString = CryptographicBuffer.EncodeToBase64String(signatureBuffer);

            var hwr = WebRequest.Create(url);

            var authorizationHeaderParams = string.Format(
                "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", oauth_token=\"{4}\", oauth_signature=\"{5}\", oauth_version=\"{6}\"",
                Uri.EscapeDataString(oauthNonce),
                Uri.EscapeDataString(OauthSignatureMethod),
                Uri.EscapeDataString(oauthTimestamp),
                Uri.EscapeDataString(oauthConsumerKey),
                Uri.EscapeDataString(oauthToken),
                Uri.EscapeDataString(signatureString),
                Uri.EscapeDataString(OauthVersion));

            hwr.Headers["Authorization"] = authorizationHeaderParams;

            var response = await hwr.GetResponseAsync();

            var reader = new StreamReader(response.GetResponseStream());
            var content = reader.ReadToEnd();

            var credentials = new TwitterCredentials
                                  {
                                      ConsumerKey = consumerKey,
                                      ConsumerSecret = consumerSecret
                                  };

            foreach (var a in content.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var index = a.IndexOf("=");
                var key = a.Substring(0, index);
                var value = a.Substring(index + 1);

                if (key.Equals("oauth_token", StringComparison.OrdinalIgnoreCase))
                    credentials.Token = value;

                if (key.Equals("oauth_token_secret", StringComparison.OrdinalIgnoreCase))
                    credentials.TokenSecret = value;
            }

            credentials.Valid = true;

            return credentials;
        }
    }
}