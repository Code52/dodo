using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BoxKite.Models;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BoxKite
{
    public class TwitterAuthenticator
    {
        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        private static String UrlEncode(String toEncode)
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
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = data;
                var response = (HttpWebResponse)await request.GetResponseAsync();
                var responseDataStream = new StreamReader(response.GetResponseStream());
                return responseDataStream.ReadToEnd();
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

            var sigBaseStringParams = "oauth_callback=" + UrlEncode(twitterCallbackUrl);
            sigBaseStringParams += "&" + "oauth_consumer_key=" + twitterClientID;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce.ToString();
            sigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + sinceEpoch;
            sigBaseStringParams += "&" + "oauth_version=1.0";
            var sigBaseString = "POST&";
            sigBaseString += UrlEncode(twitterUrl) + "&" + UrlEncode(sigBaseStringParams);

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(twitterClientSecret + "&", BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            var dataToPost = "OAuth oauth_callback=\"" + UrlEncode(twitterCallbackUrl) + "\", oauth_consumer_key=\"" + twitterClientID + "\", oauth_nonce=\"" + nonce.ToString() + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + sinceEpoch + "\", oauth_version=\"1.0\", oauth_signature=\"" + UrlEncode(signature) + "\"";

            var response = await PostData(twitterUrl, dataToPost);

            if (string.IsNullOrWhiteSpace(response))
                return TwitterCredentials.Null;
            
            string oauthToken = null;
            var keyValPairs = response.Split('&');

            foreach (var splits in keyValPairs.Select(t => t.Split('=')))
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

            var result =  await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);

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

            var authorizationHeaderParams = "OAuth ";
            authorizationHeaderParams += "oauth_nonce=\"" + Uri.EscapeDataString(oauthNonce) + "\",";
            authorizationHeaderParams += "oauth_signature_method=\"" + Uri.EscapeDataString(OauthSignatureMethod) + "\",";
            authorizationHeaderParams += "oauth_timestamp=\"" + Uri.EscapeDataString(oauthTimestamp) + "\",";
            authorizationHeaderParams += "oauth_consumer_key=\"" + Uri.EscapeDataString(oauthConsumerKey) + "\",";
            authorizationHeaderParams += "oauth_token=\"" + Uri.EscapeDataString(oauthToken) + "\",";
            authorizationHeaderParams += "oauth_signature=\"" + Uri.EscapeDataString(signatureString) + "\",";
            authorizationHeaderParams += "oauth_version=\"" + Uri.EscapeDataString(OauthVersion) + "\"";

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