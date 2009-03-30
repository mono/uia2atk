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

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace CustomControl
{
	public abstract class UselessChildControl : UserControl
	{
		protected UselessChildControl ()
		{
			Child.MouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e) {
				// Double click hack
				if ((DateTime.Now.Ticks - lastTicks) < 2310000) {
					Grid grid = (Grid) Parent;
					UselessControl parent = (UselessControl) grid.Parent;
					parent.RemoveUselessChildControl (this);
				} else {
					e.Handled = true;
					IsSelected = !IsSelected;
				}

				lastTicks = DateTime.Now.Ticks;
			};

			SizeChanged += delegate (object sender, SizeChangedEventArgs e) {
				Child.SetValue (Canvas.WidthProperty, e.NewSize.Width);
				Child.SetValue (Canvas.HeightProperty, e.NewSize.Height);
			};

			Content = Child;

			Child.SetValue (Canvas.WidthProperty, Width);
			Child.SetValue (Canvas.HeightProperty, Height);
			SetValue (UselessChildControl.ChildFillProperty, ChildFill);
		}

		public event EventHandler<EventArgs> Selected;

		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected == value)
					return;

				isSelected = value;
				OnSelected ();
			}
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
			return new UselessChildControlPeer (this);
		}

		#region ChildFill Property and Dependency Property

		public SolidColorBrush ChildFill
		{
			get { return (SolidColorBrush) GetValue (ChildFillProperty); }
			set {
				SetValue (UselessChildControl.ChildFillProperty, value);
				Child.SetValue (Shape.FillProperty, ChildFill);
			}
		}

		public static readonly DependencyProperty ChildFillProperty
			= DependencyProperty.Register("ChildFill",
										   typeof (SolidColorBrush),
										   typeof (UselessChildControl),
										   new PropertyMetadata (new SolidColorBrush (Color.FromArgb (255, 0, 0, 0))));

		public static SolidColorBrush GetChildFill (UIElement element)
		{
			return (SolidColorBrush) element.GetValue (UselessChildControl.ChildFillProperty);
		}

		public static void SetChildFill (UIElement element, SolidColorBrush brush)
		{
			element.SetValue (UselessChildControl.ChildFillProperty, brush);
		}

		#endregion

		#region ChildSelectedFill Property and Dependency Property

		public SolidColorBrush ChildSelectedFill {
			get { return (SolidColorBrush) GetValue (ChildSelectedFillProperty); }
			set {
				SetValue (UselessChildControl.ChildSelectedFillProperty, value);
				if (IsSelected)
					Child.SetValue (Shape.FillProperty, ChildSelectedFill);
			}
		}

		public static readonly DependencyProperty ChildSelectedFillProperty 
			= DependencyProperty.Register ("ChildSelectedFill", 
			                               typeof (SolidColorBrush),
										   typeof (UselessChildControl),
										   new PropertyMetadata (new SolidColorBrush (Color.FromArgb (255, 0, 0, 0))));

		public static SolidColorBrush GetChildSelectedFill (UIElement element)
		{
			return (SolidColorBrush) element.GetValue (UselessChildControl.ChildSelectedFillProperty);
		}

		public static void SetChildSelectedFill (UIElement element, SolidColorBrush brush)
		{
			element.SetValue (UselessChildControl.ChildSelectedFillProperty, brush);
		}

		#endregion

		#region Protected members

		protected abstract UIElement Child {
			get;
		}

		protected void OnSelected()
		{
			if (IsSelected)
				Child.SetValue (Shape.FillProperty, ChildSelectedFill);
			else
				Child.SetValue (Shape.FillProperty, ChildFill);

			if (Selected != null)
				Selected (this, EventArgs.Empty);
		}

		#endregion

		bool isSelected;
		long lastTicks = 0; 
	}
}
