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
//	Neville Gao <nevillegao@gmail.com>
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
    	public class ProgressBarProviderTest : BaseProviderTest
    	{
		#region Test

		[Test]
		public void BasicPropertiesTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.ProgressBar.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "progress bar");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			object rangeValueProvider =
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			Assert.IsTrue (rangeValueProvider is IRangeValueProvider,
			               "Not returning RangeValuePatternIdentifiers.");
		}

		#endregion

		#region IRangeValuePattern Test

		[Test]
		public void IRangeValueProviderValueTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			double value = (double) progressBar.Value;
			Assert.AreEqual (rangeValueProvider.Value, value, "Value value");
		}

		[Test]
		public void IRangeValueProviderIsReadOnlyTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");

			Assert.AreEqual (rangeValueProvider.IsReadOnly,
			                 true,
			                 "IsReadOnly value");
		}
		
		[Test]
		public void IRangeValueProviderMinimumTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");

			Assert.AreEqual (rangeValueProvider.Minimum,
			                 0.0,
			                 "Minimum value");
		}
		
		[Test]
		public void IRangeValueProviderMaximumTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");

			Assert.AreEqual (rangeValueProvider.Maximum,
			                 100.0,
			                 "Maximum value");
		}
		
		[Test]
		public void IRangeValueProviderLargeChangeTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);

			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");

			Assert.AreEqual (rangeValueProvider.LargeChange,
			                 Double.NaN,
			                 "LargeChange value");
		}
		
		[Test]
		public void IRangeValueProviderSmallChangeTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");

			Assert.AreEqual (rangeValueProvider.SmallChange,
			                 Double.NaN,
			                 "SmallChange value");
		}
		
		[Test]
		public void IRangeValueProviderSetValueTest ()
		{
			ProgressBar progressBar = CreateProgressBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (progressBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			try {
				double minValue = progressBar.Minimum - 1;
				rangeValueProvider.SetValue (minValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }
			
			try {
				double maxValue = progressBar.Maximum + 1;
				rangeValueProvider.SetValue (maxValue);
				Assert.Fail ("ArgumentOutOfRangeException not thrown.");
			} catch (ArgumentOutOfRangeException) { }
			
			double value = 50;
			rangeValueProvider.SetValue (value);
			Assert.AreEqual (value, progressBar.Value, "SetValue value");
		}
		
		#endregion


		#region BaseProviderTest Overrides

		protected override Control GetControlInstance ()
		{
			return CreateProgressBar ();
		}

		public override void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}

		#endregion

		#region Protected Methods

		protected virtual ProgressBar CreateProgressBar ()
		{
			return new ProgressBar ();
		}

		#endregion
    }
}
