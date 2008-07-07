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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	
	[TestFixture]
	public class ThumbProviderTest: BaseProviderTest
	{
		
#region Basic Tests
		
		[Test]
		public void BasicPropertiesTest ()
		{
			IRawElementProviderFragment provider = new ThumbProvider (null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.NameProperty,
			              null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LabeledByProperty,
			              null);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Thumb.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "thumb");
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsControlElementProperty,
			              true);
		}
		
		[Test]
		public override void IsEnabledPropertyTest ()
		{
			//FIXME: How to?
		}
		
		[Test]
		public override void AutomationIdPropertyTest () 
		{
			//FIXME: How to?
		}
		
		[Test]
		public override void BoundingRectanglePropertyTest ()
		{
			//FIXME: How to?
		}
		
		[Test]
		public override void ClickablePointPropertyTest ()
		{
			//FIXME: How to?
		}
		
		[Test]
		public override void IsKeyboardFocusablePropertyTest ()
		{
			//FIXME: How to?
		}

		[Test]
		public override void NamePropertyTest ()
		{
			//Name is not supported.
		}		
		
#endregion
		
		
#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return null;
		}
		
#endregion
	}
}
