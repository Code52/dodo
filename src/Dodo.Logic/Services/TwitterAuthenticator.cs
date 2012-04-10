using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Dodo.Logic.Services
{
    public class TwitterAuthenticator
    {
        private static String UrlEncode(String toEncode)
        {
            var encoded = "";

            for (var i = 0; i < toEncode.Length; i++)
            {
                var Test = toEncode[i];
                if ((Test >= 'A' && Test <= 'Z') ||
                    (Test >= 'a' && Test <= 'z') ||
                    (Test >= '0' && Test <= '9'))
                {
                    encoded += Test;
                }
                else if (Test == '-' || Test == '_' || Test == '.' || Test == '~')
                {
                    encoded += Test;
                }
                else
                {
                    encoded += "%" + String.Format("{0:X}", (int)Test);
                }
            }
            return encoded;
        }

        private static async Task<string> PostData(String Url, String Data)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Headers["Authorization"] = Data;
                var response = (HttpWebResponse)await request.GetResponseAsync();
                var responseDataStream = new StreamReader(response.GetResponseStream());
                return responseDataStream.ReadToEnd();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<WebAuthenticationResult> AuthenticateUser(string twitterClientID, string twitterCallbackUrl, string twitterClientSecret)
        {
            if (string.IsNullOrWhiteSpace(twitterClientID))
                throw new ArgumentException("TwitterClientID must be specified", twitterClientID);

            if (string.IsNullOrWhiteSpace(twitterCallbackUrl))
                throw new ArgumentException("TwitterCallbackUrl must be specified", twitterCallbackUrl);

            if (string.IsNullOrWhiteSpace(twitterClientSecret))
                throw new ArgumentException("TwitterClientSecret must be specified", twitterClientSecret);

            var sinceEpoch = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            var rand = new Random();
            var twitterUrl = "https://api.twitter.com/oauth/request_token";
            var nonce = rand.Next(1000000000);
            
            var sigBaseStringParams = "oauth_callback=" + UrlEncode(twitterCallbackUrl);
            sigBaseStringParams += "&" + "oauth_consumer_key=" + twitterClientID;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce.ToString();
            sigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(sinceEpoch.TotalSeconds);
            sigBaseStringParams += "&" + "oauth_version=1.0";
            var sigBaseString = "POST&";
            sigBaseString += UrlEncode(twitterUrl) + "&" + UrlEncode(sigBaseStringParams);

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(twitterClientSecret + "&", BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            var dataToPost = "OAuth oauth_callback=\"" + UrlEncode(twitterCallbackUrl) + "\", oauth_consumer_key=\"" + twitterClientID + "\", oauth_nonce=\"" + nonce.ToString() + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + Math.Round(sinceEpoch.TotalSeconds) + "\", oauth_version=\"1.0\", oauth_signature=\"" + UrlEncode(signature) + "\"";

            var response = await PostData(twitterUrl, dataToPost);

            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }

            String oauth_token = null;
            String oauth_token_secret = null;
            var keyValPairs = response.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                var splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        oauth_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(oauth_token))
                return null;
            
            twitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;
            var startUri = new Uri(twitterUrl);
            var endUri = new Uri(twitterCallbackUrl);

            return await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
        }
    }
}