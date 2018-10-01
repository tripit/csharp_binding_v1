/*
 * Copyright 2008-2018 Concur Technologies, Inc.
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
using System.Net;

namespace TripIt.Example {

    class GetAuthorizedToken {
        public static void Main(string[] args) {
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatesPolicy();
            
            if (args.Length < 5) {
                Console.WriteLine("Usage: GetAuthorizedToken.exe api_url consumer_key " +
                    "consumer_secret request_token " +
                    "request_token_secret");
                return;
            }
            
            string apiUrl = args[0];
            string consumerKey = args[1];
            string consumerSecret = args[2];
            string requestToken = args[3];
            string requestTokenSecret = args[4];
            
            Credential credential = new OAuthCredential(
                consumerKey, consumerSecret,
                requestToken, requestTokenSecret);
            
            TripIt t = new TripIt(credential, apiUrl);
            
            OAuthCredential authorizedCredential = t.GetAccessToken();
            
            Console.WriteLine("Authorized Token: " + authorizedCredential.Token);
            Console.WriteLine("Token Secret:     " + authorizedCredential.TokenSecret);
        }
    }
    
}
