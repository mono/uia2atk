using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace DockProviderTest
{
	public class DockProviderForm : Form
	{
		public DockProviderForm ()
		{
			Controls.Add (new DockProviderLabel ());
		}

		[STAThread]
		static void Main ()
		{
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			Application.Run (new DockProviderForm ());
		}
	}

	public class DockProviderLabel : Label, IRawElementProviderSimple, IDockProvider
	{
		public DockProviderLabel ()
			: base ()
		{
			Dock = DockStyle.Top;
			Text = "Top";
			BackColor = Color.Black;
			ForeColor = Color.White;
		}

		protected override void WndProc (ref Message m)
		{
			if (m.Msg == WM_GETOBJECT
                            && (int)m.LParam == AutomationInteropProvider.RootObjectId) {
				m.Result = AutomationInteropProvider.ReturnRawElementProvider (
					this.Handle, m.WParam, m.LParam,
					(IRawElementProviderSimple) this);
				return;
			}

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

		public IRawElementProviderSimple HostRawElementProvider
		{
			get { return this; }
		}

		public ProviderOptions ProviderOptions
		{
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

			this.Invoke ((MethodInvoker) delegate () {
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
				AutomationInteropProvider.RaiseAutomationPropertyChangedEvent(this,
					new AutomationPropertyChangedEventArgs (DockPattern.DockPositionProperty,
					                                        oldValue, DockPosition));
				AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (this,
					new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.NameProperty,
										oldValue.ToString (), DockPosition.ToString ()));
			}
		}
#endregion

#region Private Fields
		private const int WM_GETOBJECT = 0x003D;
#endregion
	}
}
