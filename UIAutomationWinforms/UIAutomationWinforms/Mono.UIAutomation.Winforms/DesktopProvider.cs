using System;
using System.ComponentModel;
using System.Windows.Forms;

using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	//
	// This semi-provider -- there is no real "Desktop" control.
	// This class helps to deal with providers tree.
	//
	[MapsComponent (typeof (DesktopComponent))]
	internal class DesktopProvider : PaneProvider
	{
		public DesktopProvider (Component component) : base (component)
		{
		}
	}
}
