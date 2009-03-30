using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CustomControl
{
	public class UselessRectangleControl : UselessChildControl
	{
		public UselessRectangleControl ()
		{
		}

		protected override UIElement Child {
			get {
				if (rectangle == null)
					rectangle = new Rectangle ();
				return rectangle;
			}
		}

		Rectangle rectangle;
	}
}
