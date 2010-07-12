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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
//
// Authors:
//      Mario Carrion <mcarrion@novell.com>
//

using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class FragmentControlProviderWrapperTest : BaseProviderTest
	{
		#region BaseProviderTest Overrides

		protected override bool IsContentElement {
			get { return true; }
		}

		protected override Control GetControlInstance ()
		{
			return new DockProviderLabel ();
		}

		public override void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			TestProperty (provider, AutomationElementIdentifiers.IsKeyboardFocusableProperty, false);
		}

		// We don't require these
		public override void AmpersandsAndNameTest () { }
		public override void LabeledByAndNamePropertyTest () { }

		#endregion

		[Test]
		public void ProviderPatternTest ()
		{
			DockProviderLabel label = new DockProviderLabel ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (label);

			object dockPattern = provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (dockPattern, "DockPattern.");

			IDockProvider dockProvider = dockPattern as IDockProvider;
			Assert.IsNotNull (dockProvider, "IDockProvider ");

			Assert.AreEqual (provider.HostRawElementProvider, label, "HostRawElementProvider");
			Assert.AreEqual (ProviderOptions.ServerSideProvider, provider.ProviderOptions, "ProviderOptions");

			Assert.AreEqual (DockStyle.Top.ToString (),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
			                 "NameProperty");

			dockProvider.SetDockPosition (DockPosition.Right);
			Assert.AreEqual (DockStyle.Right.ToString (),
			                 provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id),
			                 "NameProperty");
		}

		public class DockProviderLabel : Label, IRawElementProviderSimple, IDockProvider
		{
			private const int WM_GETOBJECT = 0x003D;

			public DockProviderLabel (): base ()
			{
				Dock = DockStyle.Top;
				Text = "Top";
				BackColor = Color.Black;
				ForeColor = Color.White;
			}

			protected override void WndProc (ref Message m)
			{
				if (m.Msg == WM_GETOBJECT
				    && (int) m.LParam == AutomationInteropProvider.RootObjectId) {
					m.Result = AutomationInteropProvider.ReturnRawElementProvider (
					           this.Handle, m.WParam, m.LParam,
					           (IRawElementProviderSimple) this);
				} else
					base.WndProc (ref m);
			}

			#region IRawElementProviderSimple Members
			public object GetPatternProvider (int patternId)
			{
				if (patternId == DockPattern.Pattern.Id)
					return this;

				return null;
			}

			public object GetPropertyValue (int propertyId)
			{
				if (propertyId == DockPattern.DockPositionProperty.Id)
					return DockPosition;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return DockPosition.ToString ();
				return null;
			}

			public IRawElementProviderSimple HostRawElementProvider {
				get { return this; }
			}

			public ProviderOptions ProviderOptions {
				get { return ProviderOptions.ServerSideProvider; }
			}
			#endregion

			#region IDockProvider Members
			public DockPosition DockPosition {
				get;
				private set;
			}

			public void SetDockPosition (DockPosition dockPosition)
			{
				DockPosition oldValue = DockPosition;
				DockPosition = dockPosition;

				this.Invoke ((MethodInvoker) delegate() {
					switch (dockPosition) {
					case DockPosition.Top:
						Dock = DockStyle.Top;
						break;
					case DockPosition.Bottom:
						Dock = DockStyle.Bottom;
						break;
					case DockPosition.Left:
						Dock = DockStyle.Left;
						break;
					case DockPosition.Right:
						Dock = DockStyle.Right;
						break;
					case DockPosition.Fill:
						Dock = DockStyle.Fill;
						break;
					default:
						Dock = DockStyle.None;
						break;
					}
					Text = Dock.ToString ();
				});

				if (AutomationInteropProvider.ClientsAreListening) {
					AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (this,
						new AutomationPropertyChangedEventArgs (DockPattern.DockPositionProperty,
							oldValue, DockPosition));
					AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (this,
						new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.NameProperty,
							oldValue.ToString (), DockPosition.ToString ()));
				}
			}

			#endregion
		}

	}
}

