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
using System.Text;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TripIt {
 
    internal static class Utility {
        
        public static string UrlEncode(string str) {
            StringBuilder sb = new StringBuilder();
            
            foreach (byte b in Encoding.UTF8.GetBytes(str)) {
                if ((b >= 'A' && b <= 'Z') ||
                    (b >= 'a' && b <= 'z') ||
                    (b >= '0' && b <= '9') ||
                    b == '_' || b == '.' || b == '-' || b == '~')
                {
                    sb.Append((char)b);
                }
                else {
                    sb.Append('%');
                    sb.Append(b.ToString("X2"));
                }
            }
            
            return sb.ToString();
        }
        
        public static string UrlEncodeArgs(Dictionary<string, string> args) {
            return String.Join("&",
                (from arg in args
                    select UrlEncode(arg.Key) + "=" + UrlEncode(arg.Value)
                ).ToArray());
        }
		
		public static string JsonEncode(string str) {
			StringBuilder sb = new StringBuilder();
			
			sb.Append('"');
			foreach (char c in str) {
				switch (c) {
				case '"':
					sb.Append("\\\"");
					break;
				case '\\':
					sb.Append("\\\\");
					break;
				case '\b':
					sb.Append("\\b");
					break;
				case '\f':
					sb.Append("\\f");
					break;
				case '\n':
					sb.Append("\\n");
					break;
				case '\r':
					sb.Append("\\r");
					break;
				case '\t':
					sb.Append("\\t");
					break;
				default:
					sb.Append(c);
					break;
				}
			}
			sb.Append('"');
			
			return sb.ToString();
		}
        
    }
    
}