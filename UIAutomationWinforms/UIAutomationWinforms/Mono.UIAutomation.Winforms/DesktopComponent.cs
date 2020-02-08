using System;
using System.ComponentModel;

namespace Mono.UIAutomation.Winforms
{
	internal class DesktopComponent : Component
	{
		private static DesktopComponent instance = null;

		private DesktopComponent ()
		{
		}

		public static DesktopComponent Instance
		{
			get
			{
				if (instance == null)
					instance = new DesktopComponent ();
				return instance;
			}
		}
	}
}
