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
using System.Collections.Generic;
using System.Xml;

namespace UiaAtkBridgeTest
{
	
	internal class EventCollection : List <AtSpiEvent>
	{
		string originalRepr = null;

		internal EventCollection (List <AtSpiEvent> initalCol, string eventsInXml) : base (initalCol) {
			this.originalRepr = eventsInXml;
		}
		
		internal EventCollection (string eventsInXml)
		{
			originalRepr = eventsInXml;
			XmlDocument xml = new XmlDocument ();
			xml.LoadXml (eventsInXml);
			if (!xml.HasChildNodes)
				throw new ArgumentException ("XML must have child nodes", eventsInXml);
			foreach (XmlElement xmlEvent in xml.GetElementsByTagName ("event")) {
				this.Add (new AtSpiEvent (xmlEvent.Attributes.GetNamedItem("source_name").Value,
				                          xmlEvent.Attributes.GetNamedItem("source_role").Value,
				                          xmlEvent.Attributes.GetNamedItem("type").Value,
				                          xmlEvent.Attributes.GetNamedItem("detail1").Value,
				                          xmlEvent.Attributes.GetNamedItem("detail2").Value,
				                          xmlEvent.InnerText));
			}
		}

		public string OriginalGrossXml
		{
			get { return originalRepr.Replace ("</event>", "</event>" + Environment.NewLine); }
		}

		public EventCollection FindByType (string type)
		{
			return new EventCollection (this.FindAll (delegate (AtSpiEvent ev) { return ev.Type == type; }), originalRepr);
		}
		
		public EventCollection FindByRole (Atk.Role role)
		{
			return new EventCollection (this.FindAll (delegate (AtSpiEvent ev) { return ev.SourceRole == role; }), originalRepr);
		}
	}
}
