// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Globalization;

namespace UiaAtkBridgeTest
{
	
	internal class AtSpiEvent
	{
		string type = null;
		string detail1 = null;
		string detail2 = null;
		string sourceName = null;
		Atk.Role sourceRole;
		string data = null;
		
		public string Type {
			get { return type; }
			set { type = value; }
		}
		
		public string SourceName {
			get { return sourceName; }
			set { sourceName = value; }
		}

		public Atk.Role SourceRole {
			get { return sourceRole; }
			set { sourceRole = value; }
		}
		
		public string Detail2 {
			get { return detail2; }
			set { detail2 = value; }
		}
		
		public string Detail1 {
			get { return detail1; }
			set { detail1 = value; }
		}

		public string Data {
			get { return data; }
			set { data = value; }
		}
		
		internal AtSpiEvent(string sourceName, string sourceRole, string type, string detail1, string detail2, string data)
		{
			this.type = type;
			this.detail1 = detail1;
			this.detail2 = detail2;
			this.sourceName = sourceName;
			this.sourceRole = FromName (sourceRole);
			this.data = data;
		}

		internal static Atk.Role FromName (string role)
		{
			string newRole = String.Empty;
			string[] words = role.Split (' ');
			for (int i = 0; i < words.Length; i++)
				words [i] = words [i].Substring (0, 1).ToUpper () + words [i].Substring (1);
			for (int i = 0; i < words.Length; i++)
				newRole += words [i];
			return (Atk.Role) Enum.Parse (typeof (Atk.Role), newRole);
		}
	}
}
