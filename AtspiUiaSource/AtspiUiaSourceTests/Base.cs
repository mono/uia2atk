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
//      Mike Gorse <mgorse@novell.com>
//


using System;
using System.Collections.Generic;
using NUnit.Framework;
using AtspiUiaSource;
using Mono.UIAutomation.Source;

namespace AtspiUiaSourceTest
{
	public abstract class Base
	{
		#region Protected Fields
		protected AutomationSource source;
		#endregion
		
		#region Private Fields
		
		private static System.Diagnostics.Process p = null;

		#endregion
		
		#region Setup/Teardown

		public Base ()
		{
			source = new AutomationSource ();
		}

		public virtual void StartApplication (string name)
		{
			if (p != null)
				return;
			p = new System.Diagnostics.Process ();
			p.StartInfo.FileName = "python";
			p.StartInfo.Arguments = name;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.Start ();
			System.Threading.Thread.Sleep (1000);
		}
		
		[TestFixtureTearDown]
		public virtual void TearDown ()
		{
			if (p != null)
				p.Kill ();
			p = null;
		}

#endregion

		#region Private Methods

		public static string GetAppPath (string filename)
		{
			return "../../" + filename;
		}
#endregion

		#region Protected Helper Methods
		
		protected IElement GetApplication (string filename)
		{
			return GetApplication (filename, filename);
		}

		protected IElement GetApplication (string filename, string name)
		{
			if (filename != null)
				StartApplication (GetAppPath (filename));
			source.Initialize ();
			return FindApplication (name);
		}

		protected IElement FindApplication (string name)
		{
			IElement [] elements = source.GetRootElements ();
			foreach (IElement e in elements)
				if (e.Name == name)
					return e;
			return null;
		}
		#endregion

		#region Abstract Members
	
		#endregion
	}
	
}
