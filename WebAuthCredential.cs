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
 
 using System.Net;
 using System.Collections.Generic;
 
 namespace TripIt {
 
    /// <summary>
    /// A Credential that handles web authentication with a username
    /// and password.
    /// </summary>
    public class WebAuthCredential : Credential {
        private string _username;
        private string _password;
    
        /// <summary>
        /// Creates a web authorization credential with a username and
        /// a password.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public WebAuthCredential(string username, string password) {
            _username = username;
            _password = password;
        }
    
        /// <summary>
        /// Authorize an web authorization request.
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
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(_username, _password);
        }
    }
    
}