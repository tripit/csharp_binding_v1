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
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TripIt {

    /// <summary>
    /// The TripIt API. Create an instance of this with a credential
    /// and start calling methods. The API methods generally return
    /// Response objects, which are parsed versions of the XML.
    /// See the TripIt API documentation for details of the objects
    /// available.
    /// </summary>
    public class TripIt {
        private const string API_VERSION = "v1";
        
        private Credential _credential;
        private string _apiUrl;
    
        /// <summary>
        /// Create a TripIt API object given a credential.
        /// </summary>
        /// <param name="credential">
        /// A <see cref="Credential"/>
        /// </param>
        public TripIt(Credential credential)
            : this(credential, "https://api.tripit.com")
        {}
        
        /// <summary>
        /// Create a TripIt API object given a credential and an
        /// API URL. For developers external to TripIt, the API
        /// URL should generally be omitted. The default points at the
        /// public API URL.
        /// </summary>
        /// <param name="credential">
        /// A <see cref="Credential"/>
        /// </param>
        /// <param name="apiUrl">
        /// An API URL.
        /// </param>
        public TripIt(Credential credential, string apiUrl) {
            _credential = credential;
            _apiUrl = apiUrl;
        }
        
        /// <value>
        /// The current Credential.
        /// </value>
        public Credential Credential {
            get {
                return _credential;
            }
        }
        
        /// <summary>
        /// For OAuth clients, retrieve a request token. If this is
        /// successful, it will set the current credential to include the
        /// new request token. It will also return the new credential.
        /// </summary>
        /// <returns>
        /// The new <see cref="OAuthCredential"/>, which includes the request
        /// token.
        /// </returns>
        public OAuthCredential GetRequestToken() {
            return GetOAuthToken("request");
        }
        
        /// <summary>
        /// For OAuth clients, retrieve an access token, given a
        /// valid and authorized request token. If this is successful,
        /// it will set the current credential to include the new access
        /// token. It will also return the new credential.
        /// </summary>
        /// <returns>
        /// The new <see cref="OAuthCredential"/>, which includes the access
        /// token.
        /// </returns>
        public OAuthCredential GetAccessToken() {
            return GetOAuthToken("access");
        }
        
        private OAuthCredential GetOAuthToken(string type) {
            OAuthCredential credential = _credential as OAuthCredential;
            
            if (credential == null) {
                return null;
            }
            
            Dictionary<string, string> token = DoQueryStringRequest(
                "/oauth/" + type + "_token", null, null, null);
            
            credential = new OAuthCredential(
                credential.ConsumerKey,
                credential.ConsumerSecret,
                token["oauth_token"],
                token["oauth_token_secret"]);
            
            _credential = credential;
            return credential;
        }
        
        /// <summary>
        /// Get a Trip with the given ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the Trip to get.
        /// </param>
        /// <returns>
        /// A <see cref="TravelObject.Response"/> containing the requested Trip.
        /// </returns>
        public TravelObject.Response GetTrip(int id) {
            return GetTrip(id, new Dictionary<string, string>());
        }
        
        /// <summary>
        /// Get a Trip with the given ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the Trip to get.
        /// </param>
        /// <param name="filter">
        /// A <see cref="Dictionary"/> containing any additional filter
        /// parameters. Currently setting "include_objects" to "true" will
        /// include all objects that are associated with the requested trip
        /// in the response.
        /// </param>
        /// <returns>
        /// A <see cref="TravelObject.Response"/>
        /// </returns>
        public TravelObject.Response GetTrip(int id, Dictionary<string, string> filter) {
            filter.Add("id", id.ToString());
            return DoRequest("get", "trip", filter, null);
        }
        
        public TravelObject.Response GetAir(int id) {
            return DoRequest("get", "air", IdFilter(id), null);
        }
        
        public TravelObject.Response GetLodging(int id) {
            return DoRequest("get", "lodging", IdFilter(id), null);
        }
        
        public TravelObject.Response GetCar(int id) {
            return DoRequest("get", "car", IdFilter(id), null);
        }
        
        public TravelObject.Response GetProfile() {
            return DoRequest("get", "profile", null, null);
        }
        
        public TravelObject.Response GetRail(int id) {
            return DoRequest("get", "rail", IdFilter(id), null);
        }
        
        public TravelObject.Response GetTransport(int id) {
            return DoRequest("get", "transport", IdFilter(id), null);
        }
        
        public TravelObject.Response GetCruise(int id) {
            return DoRequest("get", "cruise", IdFilter(id), null);
        }
        
        public TravelObject.Response GetRestaurant(int id) {
            return DoRequest("get", "restaurant", IdFilter(id), null);
        }
        
        public TravelObject.Response GetActivity(int id) {
            return DoRequest("get", "activity", IdFilter(id), null);
        }
        
        public TravelObject.Response GetNote(int id) {
            return DoRequest("get", "note", IdFilter(id), null);
        }
        
        public TravelObject.Response GetMap(int id) {
            return DoRequest("get", "map", IdFilter(id), null);
        }
        
        public TravelObject.Response GetDirections(int id) {
            return DoRequest("get", "directions", IdFilter(id), null);
        }
		
		public TravelObject.Response GetPointsProgram(int id) {
			return DoRequest("get", "points_program", IdFilter(id), null);
		}
        
        public TravelObject.Response DeleteTrip(int id) {
            return DoRequest("delete", "trip", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteAir(int id) {
            return DoRequest("delete", "air", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteLodging(int id) {
            return DoRequest("delete", "lodging", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteCar(int id) {
            return DoRequest("delete", "car", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteRail(int id) {
            return DoRequest("delete", "rail", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteTransport(int id) {
            return DoRequest("delete", "transport", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteCruise(int id) {
            return DoRequest("delete", "cruise", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteRestaurant(int id) {
            return DoRequest("delete", "restaurant", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteActivity(int id) {
            return DoRequest("delete", "activity", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteNote(int id) {
            return DoRequest("delete", "note", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteMap(int id) {
            return DoRequest("delete", "map", IdFilter(id), null);
        }
        
        public TravelObject.Response DeleteDirections(int id) {
            return DoRequest("delete", "directions", IdFilter(id), null);
        }
        
        public TravelObject.Response ReplaceTrip(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "trip", null, filter);
        }
        
        public TravelObject.Response ReplaceAir(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "air", null, filter);
        }
        
        public TravelObject.Response ReplaceLodging(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "lodging", null, filter);
        }
        
        public TravelObject.Response ReplaceCar(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "car", null, filter);
        }
        
        public TravelObject.Response ReplaceRail(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "rail", null, filter);
        }
        
        public TravelObject.Response ReplaceTransport(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "transport", null, filter);
        }
        
        public TravelObject.Response ReplaceCruise(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "cruise", null, filter);
        }
        
        public TravelObject.Response ReplaceRestaurant(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "restaurant", null, filter);
        }
        
        public TravelObject.Response ReplaceActivity(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "activity", null, filter);
        }
        
        public TravelObject.Response ReplaceNote(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "note", null, filter);
        }
        
        public TravelObject.Response ReplaceMap(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "map", null, filter);
        }
        
        public TravelObject.Response ReplaceDirections(int id, string xml) {
            Dictionary<string, string> filter = IdFilter(id);
            filter.Add("xml", xml);
            return DoRequest("replace", "directions", null, filter);
        }
        
        public TravelObject.Response ListTrip() {
            return ListTrip(null);
        }
        
        public TravelObject.Response ListTrip(Dictionary<string, string> filter) {
            return DoRequest("list", "trip", filter, null);
        }
        
        public TravelObject.Response ListObject() {
            return ListObject(null);
        }
        
        public TravelObject.Response ListObject(Dictionary<string, string> filter) {
            return DoRequest("list", "object", filter, null);
        }
		
		public TravelObject.Response ListPointsProgram() {
			return DoRequest("list", "points_program", null, null);
		}
        
        /// <summary>
        /// Create an object.
        /// </summary>
        /// <param name="xml">
        /// A <see cref="System.String"/> containing XML that represents the
        /// object to create.
        /// </param>
        /// <returns>
        /// A <see cref="TravelObject.Response"/>
        /// </returns>
        public TravelObject.Response Create(string xml) {
            Dictionary<string, string> xmlArg = new Dictionary<string, string>();
            xmlArg.Add("xml", xml);
            return DoRequest("create", null, null, xmlArg);
        }
		
		public TravelObject.Response CrsLoadReservations(string xml) {
			return CrsLoadReservations(xml, null, null);
		}
		
		public TravelObject.Response CrsLoadReservations(string xml, string companyKey) {
			return CrsLoadReservations(xml, companyKey, null);
		}
		
		public TravelObject.Response CrsLoadReservations(string xml, Dictionary<string, string> parameters) {
			return CrsLoadReservations(xml, null, parameters);
		}
		
		public TravelObject.Response CrsLoadReservations(string xml, string companyKey, Dictionary<string, string> parameters) {
			if (parameters == null) {
				parameters = new Dictionary<string, string>();
			}
            parameters.Add("xml", xml);
			if (companyKey != null) {
				parameters.Add("company_key", companyKey);
			}
			
            return DoRequest("crsLoadReservations", null, null, parameters);
		}
		
		public TravelObject.Response CrsDeleteReservations(string recordLocator) {
			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add("record_locator", recordLocator);
			return DoRequest("crsDeleteReservations", null, args, null);
		}
        
        private Dictionary<string, string> IdFilter(int id) {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("id", id.ToString());
            return filter;
        }

        private TravelObject.Response DoRequest(
            string verb,
            string entity,
            Dictionary<string, string> urlArgs,
            Dictionary<string, string> postArgs)
        {
            return DeserializeResponse(
                DoRawRequest(verb, entity, urlArgs, postArgs));
        }
        
        private Dictionary<string, string> DoQueryStringRequest(
            string verb,
            string entity,
            Dictionary<string, string> urlArgs,
            Dictionary<string, string> postArgs)
        {
            return ParseQueryString(DoRawRequest(verb, entity, urlArgs, postArgs).ReadToEnd());
        }
        
        private Dictionary<string, string> ParseQueryString(string qs) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            foreach (string itemStr in qs.Split("&".ToCharArray())) {
                string[] item = itemStr.Split("=".ToCharArray(), 2);
                if (item.Length > 1) {
                    result.Add(item[0], item[1]);
                }
            }
            
            return result;
        }
        
        private TextReader DoRawRequest(
            string verb,
            string entity,
            Dictionary<string, string> urlArgs,
            Dictionary<string, string> postArgs)
        {
            string urlBase = null;
            
            if ((new string[] {
                "/oauth/request_token",
                "/oauth/access_token"} as IList).Contains(verb))
            {
                urlBase = _apiUrl + verb; 
            }
            else {
                if (entity != null) {
                    urlBase = _apiUrl + "/" + API_VERSION + "/" + verb + "/" +
                        entity;
                }
                else {
                    urlBase = _apiUrl + "/" + API_VERSION + "/" + verb;
                }
            }
            
            String url;
            Dictionary<string, string> args = null;
            if (urlArgs != null) {
                args = urlArgs;
                url = urlBase + "?" + Utility.UrlEncodeArgs(urlArgs);
            }
            else {
                url = urlBase;
            }
            
            WebRequest request = WebRequest.Create(url);
            if (postArgs != null) {
                args = postArgs;
                request.Method = "POST";
            }
            else {
                request.Method = "GET";
            }
            
            _credential.Authorize(request, _apiUrl, urlBase, args);
            
            if (postArgs != null) {
                // Add postArgs as request's post data.
                // Must happen after Authorize because we can't write a
                // request body before adding the Authorization header.
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.UTF8.GetBytes(
                    Utility.UrlEncodeArgs(postArgs));
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }
			
			WebResponse response = request.GetResponse();
            
            return new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        }
        
        private static TravelObject.Response DeserializeResponse(TextReader reader) {
            XmlSerializer serializer = new XmlSerializer(typeof(TravelObject.Response));
            return serializer.Deserialize(reader) as TravelObject.Response;
        }

    }
    
}