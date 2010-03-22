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

using Atk;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

using System.Reflection;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (AutomationControlType.Thumb,
	                    Provides=PatternInterface.Transform,
	                    ElementType="System.Windows.Automation.Peers.GridSplitterAutomationPeer")]
	public class GridSplitter : BasePatternImplementor, Atk.ValueImplementor
	{
#region Public Methods
		public GridSplitter (Adapter adapter, AutomationPeer peer) : base (adapter, peer)
		{
			transformProvider
				= (ITransformProvider) peer.GetPattern (PatternInterface.Transform);

			FrameworkElementAutomationPeer feaPeer = peer as FrameworkElementAutomationPeer;

			Type type = feaPeer.Owner.GetType ();
			if (type.Namespace != "System.Windows.Controls" || type.Name != "GridSplitter")
				return;

			gridsplitter = feaPeer.Owner as FrameworkElement;
			if (gridsplitter == null || gridsplitter.Parent == null)
				return;
			Type gridType = gridsplitter.Parent.GetType ();
			if (gridType.Namespace != "System.Windows.Controls" || gridType.Name != "Grid")
				return;

			// SWC.Grid properties
			gridRowDefinitions = GetPropertyValue<object> (gridsplitter.Parent,
			                                              "RowDefinitions");
			gridColumnDefinitions = GetPropertyValue<object> (gridsplitter.Parent,
			                                                 "ColumnDefinitions");

			// SWG.Grid attached properties
			if (GridColumnProperty == null) {
				GridColumnProperty = GetStaticField<DependencyProperty> (gridType,
				                                                        "ColumnProperty");
				GridRowProperty = GetStaticField<DependencyProperty> (gridType,
				                                                     "RowProperty");
			}

			column = (int) gridsplitter.GetValue (GridColumnProperty);
			row = (int) gridsplitter.GetValue (GridRowProperty);
		}

		public override Role OverriddenRole {
			get { return Atk.Role.SplitPane; }
		}

		// This adapter uses 0 as minimum and 100 as maximum, and its CurrentValue is
		// converted to represent a percentage.

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (0);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (100);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (0);
		}

		// Getting a percentage based on the size of the first section
		public void GetCurrentValue (ref GLib.Value value)
		{
			if (gridsplitter == null)
				value = new GLib.Value (0);
			else
				value = new GLib.Value ((GetSectionSize (0) * 100) / TotalSize);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			if (gridsplitter == null)
				return false;

			int realValue = (int) value.Val;
			if (realValue < 0 || realValue > 100)
				return false;

			int x = 0;
			int y = 0;
			int newValue = ((realValue * TotalSize) / 100);
			// Rows
			if (gridsplitter.HorizontalAlignment == HorizontalAlignment.Stretch)
				y = newValue - GetSectionSize (0);
			// Columns
			else if (gridsplitter.VerticalAlignment == VerticalAlignment.Stretch)
				x = newValue - GetSectionSize (0);

			try {
				transformProvider.Move (x, y);
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
				return false;
			}

			return true;
		}
#endregion

#region Private Members

		private int TotalSize {
			get { return GetSectionSize (0) + GetSectionSize (1); }
		}

		private int GetSectionSize (int index)
		{
			if (row == 0 || column == 0)
				return 0;

			// Use by Rows
			if (gridsplitter.HorizontalAlignment == HorizontalAlignment.Stretch) {
				int count = GetPropertyValue<int> (gridRowDefinitions, "Count");
				if (row > count)
					return 0;
				else {
					object definition = GetIndexedProperty<object> (gridRowDefinitions,
					                                               "Item",
					                                               row - (1 - index));
					return (int) GetPropertyValue<double> (definition, "ActualHeight");
				}
			// Used by Columns
			} else {
				int count = GetPropertyValue<int> (gridColumnDefinitions, "Count");
				if (column > count)
					return 0;
				else {
					object definition = GetIndexedProperty<object> (gridColumnDefinitions,
					                                               "Item",
					                                               column - (1 - index));
					return (int) GetPropertyValue<double> (definition, "ActualWidth");
				}
			}
		}
		private T GetPropertyValue<T> (object reference, string propertyName)
		{
			PropertyInfo property = reference.GetType ().GetProperty (propertyName);
			if (property != null)
				return (T) property.GetValue (reference, null);

			return default(T);
		}

		private T GetIndexedProperty<T> (object reference, string propertyName, int index)
		{
			PropertyInfo property = reference.GetType ().GetProperty (propertyName);
			if (property != null)
				return (T) property.GetValue (reference, new object [] { index });

			return default(T);
		}

		private T GetStaticField<T> (Type type, string fieldName)
		{
			FieldInfo field = type.GetField (fieldName,
			                                BindingFlags.Static | BindingFlags.Public);
			if (field != null)
				return (T) field.GetValue (null);

			return default(T);
		}

		private FrameworkElement gridsplitter;
		private object gridRowDefinitions;
		private object gridColumnDefinitions;
		private int column;
		private int row;
		private ITransformProvider transformProvider;

#endregion

#region Static Members

		// These are pointers to the attached properties defined in SWC.Grid
		private static DependencyProperty GridColumnProperty;
		private static DependencyProperty GridRowProperty;

#endregion
	}

}
