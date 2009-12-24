// Utils.cs: Utility functions
//
// This program is free software; you can redistribute it and/or modify it under
// the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Felicia Mu  (fxmu@novell.com)
//	Ray Wang  (rawang@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Mono.UIAutomation.TestFramework
{
	class Utils
	{
		//Takes a screenshot of the current desktop
		public static void TakeScreenshot (string path)
		{
			// pause before taking screenshots, otherwise we get half-drawn widgets
			//Thread.Sleep ((int) Config.Instance.ShortDelay * 1000);

			// do the screenshot, and save it to disk
			int width = Screen.PrimaryScreen.Bounds.Width;
			int height = Screen.PrimaryScreen.Bounds.Height;
			Bitmap bmp = new Bitmap (width, height);
			using (Graphics g = Graphics.FromImage (bmp)) {
				g.CopyFromScreen (0, 0, 0, 0, new Size (width, height));
			}
			bmp.Save (path, ImageFormat.Png);
		}

		// runs until it gets True.
		public static bool RetryUntilTrue (Func<bool> d)
		{
			for (int i = 0; i < Config.Instance.RetryTimes; i++) {
				if (d ())
					return true;

				Thread.Sleep (Config.Instance.RetryInterval);
			}

			return false;
		}
	}
}