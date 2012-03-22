/*
 * Copyright 2008-2012 Concur Technologies, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */
 
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TripIt {

    /// <summary>
    /// A Credential that handles OAuth.
    /// </summary>
    public class OAuthCredential : Credential {
        private const string OAUTH_SIGNATURE_METHOD = "HMAC-SHA1";
        private const string OAUTH_VERSION = "1.0";
    
        private string _consumerKey;
        private string _consumerSecret;
        private string _token;
        private string _tokenSecret;
		private string _requestorId;
		private int? _accountId;
    
        /// <summary>
        /// Creates an OAuthCredential with only a consumer key and secret.
        /// This credential would be used to get a request token.
        /// </summary>
        /// <param name="consumerKey">
        /// The consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer key secret.
        /// </param>
        public OAuthCredential(string consumerKey, string consumerSecret)
            : this(consumerKey, consumerSecret, null, null)
        {}
        
        /// <summary>
        /// Creates an OAuthCredential with a consumer key and secret, along
        /// with a token and token secret. The token may be either a request
        /// token or an authenticated token.
        /// </summary>
        /// <param name="consumerKey">
        /// The consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer key secret.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="tokenSecret">
        /// The token secret.
        /// </param>
        public OAuthCredential(string consumerKey, string consumerSecret, string token, string tokenSecret) {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = token;
            _tokenSecret = tokenSecret;
			_requestorId = null;
			_accountId = null;
        }
		
		/// <summary>
		/// Creates an OAuthCredential for 2-legged authentication.
		/// </summary>
		/// <param name="consumerKey">
		/// The consumer key.
		/// </param>
		/// <param name="consumerSecret">
		/// The consumer key secret.
		/// </param>
		/// <param name="requestorId">
		/// The identity of the requestor. Typically, this is the email address.
		/// </param>
		public OAuthCredential(string consumerKey, string consumerSecret, string requestorId) {
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_requestorId = requestorId;
			_token = _tokenSecret = null;
			_accountId = null;
		}
		
		public OAuthCredential(string consumerKey, string consumerSecret, int accountId) {
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_accountId = accountId;
			_token = _tokenSecret = null;
			_requestorId = null;
		}
		
        /// <value>
        /// The consumer key.
        /// </value>
        public string ConsumerKey {
            get {
                return _consumerKey;
            }
        }
        
        /// <value>
        /// The consumer key secret.
        /// </value>
        public string ConsumerSecret {
            get {
                return _consumerSecret;
            }
        }
        
        /// <value>
        /// The token, or null if there is none.
        /// </value>
        public string Token {
            get {
                return _token;
            }
        }
        
        /// <value>
        /// The token secret, or null if there is none.
        /// </value>
        public string TokenSecret {
            get {
                return _tokenSecret;
            }
        }
		
		/// <value>
		/// The ID of the requestor, or null if there is none.
		/// </value>
		public string RequestorId {
			get {
				return _requestorId;
			}
		}
		
		/// <value>
		/// The account ID of the requesting account.
		/// </value>
		public int? AccountId {
			get {
				return _accountId;
			}
		}
		
		/// <summary>
		/// Validates that a given signature was created with this OAuth secret.
		/// </summary>
		/// <param name="uri">
		/// The complete URI that was requested, including the oauth_signature parameter.
		/// </param>
		/// <returns>
		/// True if the signature is valid. False if it is invalid.
		/// </returns>
		public bool ValidateSignature(Uri uri) {
			string urlBase = uri.GetComponents(UriComponents.AbsoluteUri ^ UriComponents.Query, UriFormat.Unescaped);
			NameValueCollection argsCollection = HttpUtility.ParseQueryString(uri.Query);
			Dictionary<string, string> args = new Dictionary<string, string>();
			foreach (string name in argsCollection) {
				if (name != null) {
					args.Add(name, argsCollection.Get(name));
				}
			}
			
			string signature;
			if (!args.TryGetValue("oauth_signature", out signature)) {
				return false;
			}
			
			return (signature == GenerateSignature("GET", urlBase, args));
		}
		
        /// <summary>
        /// Authorize an OAuth request.
        /// </summary>
        /// <param name="request">
        /// A <see cref="WebRequest"/>
        /// </param>
        /// <param name="realm">
        /// The URL of the API, ex: https://api.tripit.com
        /// </param>
        /// <param name="urlBase">
        /// The base URL of the request, ex: https://api.tripit.com/list/trip
        /// </param>
        /// <param name="args">
        /// A <see cref="Dictionary"/> of the parameters to the request.
        /// </param>
        public void Authorize(
            WebRequest request,
            string realm,
            string urlBase,
            Dictionary<string, string> args)
        {
            request.Headers.Add("Authorization",
                GenerateAuthorizationHeader(request, realm, urlBase, args));
        }
		
		public String GetSessionParameters(String redirectUrl, String action) {
			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add("redirect_url", redirectUrl);
			IDictionary<string, string> parameters = GenerateOAuthParameters("GET", action, args);
			parameters.Add("redirect_url", redirectUrl);
			parameters.Add("action", action);			
			
			return "{" + String.Join(",", 
				(from p in parameters
					select Utility.JsonEncode(p.Key) + ":" +
			        	Utility.JsonEncode(p.Value)).ToArray()) + "}";
		}
        
        private string GenerateAuthorizationHeader(
            WebRequest request,
            string realm,
            string urlBase,
            Dictionary<string, string> args)
        {
            return "OAuth realm=\"" + realm + "\"," +
                String.Join(",",
                    (from p in GenerateOAuthParameters(request.Method, urlBase, args)
                        select Utility.UrlEncode(p.Key) + "=\"" +
                            Utility.UrlEncode(p.Value) + "\""
                    ).ToArray());
        }
        
        private IDictionary<string, string> GenerateOAuthParameters(
		    string method,
            string urlBase,
            Dictionary<string, string> args)
        {
            Dictionary<string, string> oauthParameters = new Dictionary<string, string>();
            oauthParameters.Add("oauth_consumer_key", _consumerKey);
            oauthParameters.Add("oauth_nonce", GenerateNonce());
            oauthParameters.Add("oauth_timestamp", GenerateTimestamp());
            oauthParameters.Add("oauth_signature_method", OAUTH_SIGNATURE_METHOD);
            oauthParameters.Add("oauth_version", OAUTH_VERSION);
            
            if (_token != null) {
                oauthParameters.Add("oauth_token", _token);
            }
			
			if (_requestorId != null) {
				oauthParameters.Add("xoauth_requestor_id", _requestorId);
			}
			if (_accountId.HasValue) {
				oauthParameters.Add("xoauth_account_id", _accountId.Value.ToString());
			}
			
			Dictionary<string, string> oauthParametersForBaseString = new Dictionary<string, string>(oauthParameters);
            
            if (args != null) {
                foreach (KeyValuePair<string, string> pair in args) {
                    oauthParametersForBaseString.Add(pair.Key, pair.Value);
                }
            }
            
			oauthParameters.Add("oauth_signature", GenerateSignature(method, urlBase, oauthParametersForBaseString));
            
            return oauthParameters;
        }
		
		private string GenerateSignature(string method, string urlBase, Dictionary<string, string> args) {
			string key = _consumerSecret + "&" + _tokenSecret;
            HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(GenerateSignatureBaseString(method, urlBase, args))));
		}
		
		private string GenerateSignatureBaseString(string method, string urlBase, Dictionary<string, string> args) {
			SortedDictionary<string, string> sortedArgs = new SortedDictionary<string, string>(args);
			
			sortedArgs.Remove("oauth_signature");
			
			string parameters = Utility.UrlEncode(String.Join("&",
                (from a in sortedArgs
                    select Utility.UrlEncode(a.Key) + "=" +
                        Utility.UrlEncode(a.Value)
                ).ToArray()));
			
			return method + "&" + Utility.UrlEncode(urlBase) + "&" + parameters;
		}
		
        private static string GenerateNonce() {
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            byte[] randomValue = new byte[40];
            rng.GetBytes(randomValue);
            
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(randomValue);
            StringBuilder nonce = new StringBuilder();
            foreach (byte b in data) {
                nonce.Append(b.ToString("x2"));
            }
            return nonce.ToString();
        }
        
        private static string GenerateTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
    
}
