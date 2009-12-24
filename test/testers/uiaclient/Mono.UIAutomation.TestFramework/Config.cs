// Config.cs: The default configuration
//
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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Ray Wang <rawang@novell.com>
//	Felicia Mu <fxmu@novell.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Mono.UIAutomation.TestFramework
{
	public class Config
	{
		private static Config instance = null;
		public static Config Instance {
			get {
				if (instance == null)
					ReadFromConfigureFile ();
				return instance;
			}
		}

		private static void ReadFromConfigureFile ()
		{
			var serializer = new XmlSerializer (typeof (Config));
			using (FileStream fs = new FileStream ("Config.xml", FileMode.Open)) {
				instance = serializer.Deserialize (fs) as Config;
			}
		}

		public int RetryTimes { get; set; }
		public int RetryInterval { get; set; }
		public bool TakeScreenShots { get; set; }
		// where to write procedure logger output, screenshots, etc
		public string OutputDir { get; set; }
		public int ShortDelay { get; set; }
		public int MediumDelay { get; set; }
		public int LongDelay { get; set; }
	}
}
