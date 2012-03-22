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
    /// An abstract credential that can authorize itself with a WebRequest.
    /// </summary>
    public interface Credential {
        /// <summary>
        /// Given a WebRequest and the contents of the request, modifies the
        /// request so that it
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
        void Authorize(
            WebRequest request,
            string realm,
            string urlBase,
            Dictionary<string, string> args);
    }
    
}