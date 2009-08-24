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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using SW = System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using DC = Mono.UIAutomation.UiaDbus;
using Mono.UIAutomation.UiaDbus.Interfaces;

using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbusBridge.Wrappers
{
	internal class ProviderElementWrapper : IAutomationElement
	{
#region Private Static Fields

		private static volatile int idCount = 0;
		private static object syncRoot = new object ();

#endregion

#region Private Fields

		private IRawElementProviderSimple provider;
		private IRawElementProviderFragment fragment;
		private int pathId;
		private Bus bus;
		private Dictionary<int, string> patternPathMapping = new Dictionary<int, string> ();

#endregion

#region Constructor

		public ProviderElementWrapper (IRawElementProviderSimple provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			this.provider = provider;
			fragment = provider as IRawElementProviderFragment;
			lock (syncRoot)
				pathId = ++idCount;
		}

#endregion

#region IAutomationElement Members

		public bool SupportsProperty (int propertyId)
		{
			object val = provider.GetPropertyValue (propertyId);
			return val != null && val != AEIds.NotSupported;
		}

		public string AcceleratorKey {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.AcceleratorKeyProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string AccessKey {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.AccessKeyProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string AutomationId {
			get {
				int? val = (int?)
					provider.GetPropertyValue (AEIds.AutomationIdProperty.Id);
				if (!val.HasValue)
					return string.Empty;
				return val.Value.ToString ();
			}
		}

		public DC.Rect BoundingRectangle {
			get {
				SW.Rect? val = (SW.Rect?)
					provider.GetPropertyValue (AEIds.BoundingRectangleProperty.Id);
				if (!val.HasValue)
					return new DC.Rect (SW.Rect.Empty);
				return new DC.Rect (val.Value);
			}
		}

		public string ClassName {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ClassNameProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public DC.Point ClickablePoint {
			get {
				SW.Point? val = (SW.Point?)
					provider.GetPropertyValue (AEIds.ClickablePointProperty.Id);
				if (!val.HasValue)
					return new DC.Point (new SW.Point (double.NegativeInfinity, double.NegativeInfinity));
				return new DC.Point (val.Value);
			}
		}

		public int ControlTypeId {
			get {
				int? val = (int?)
					provider.GetPropertyValue (AEIds.ControlTypeProperty.Id);
				if (!val.HasValue)
					return -1;
				return val.Value;
			}
		}

		public string FrameworkId {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.FrameworkIdProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public bool HasKeyboardFocus {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.HasKeyboardFocusProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public string HelpText {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.HelpTextProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public bool IsContentElement {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsContentElementProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsControlElement {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsControlElementProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsEnabled {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsEnabledProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsKeyboardFocusable {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsKeyboardFocusableProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsOffscreen {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsOffscreenProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsPassword {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsPasswordProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public bool IsRequiredForForm {
			get {
				bool? val = (bool?)
					provider.GetPropertyValue (AEIds.IsRequiredForFormProperty.Id);
				if (!val.HasValue)
					return false;
				return val.Value;
			}
		}

		public string ItemStatus {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ItemStatusProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string ItemType {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.ItemTypeProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string LabeledByElementPath {
			get {
				IRawElementProviderSimple labeledBy = (IRawElementProviderSimple)
					provider.GetPropertyValue (AEIds.LabeledByProperty.Id);
				if (labeledBy == null)
					return string.Empty;
				ProviderElementWrapper labeledByWrapper =
					AutomationBridge.Instance.GetProviderWrapper (labeledBy);
				if (labeledByWrapper == null)
					return string.Empty;
				return labeledByWrapper.Path;
			}
		}

		public string LocalizedControlType {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.LocalizedControlTypeProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public string Name {
			get {
				string val = (string)
					provider.GetPropertyValue (AEIds.NameProperty.Id);
				if (val == null)
					return string.Empty;
				return val;
			}
		}

		public int NativeWindowHandle {
			get {
				IntPtr? val = (IntPtr?)
					provider.GetPropertyValue (AEIds.NativeWindowHandleProperty.Id);
				if (!val.HasValue)
					return -1;
				return val.Value.ToInt32 ();
			}
		}

		public OrientationType Orientation {
			get {
				OrientationType? val = (OrientationType?)
					provider.GetPropertyValue (AEIds.OrientationProperty.Id);
				if (!val.HasValue)
					return OrientationType.None;
				return val.Value;
			}
		}

		public int ProcessId {
			get {
				int? val = (int?)
					provider.GetPropertyValue (AEIds.ProcessIdProperty.Id);
				if (!val.HasValue)
					return -1;
				return val.Value;
			}
		}

		public int [] RuntimeId {
			get {
				int [] val = (int [])
					provider.GetPropertyValue (AEIds.RuntimeIdProperty.Id);
				if (val == null)
					return new int [0];
				return val;
			}
		}

		public string ParentElementPath {
			get {
				if (fragment == null)
					return string.Empty;
				var parent = fragment.Navigate (NavigateDirection.Parent);
				if (parent == null || parent == fragment)
					return string.Empty;
				ProviderElementWrapper parentWrapper =
					AutomationBridge.Instance.GetProviderWrapper (parent);
				if (parentWrapper == null)
					return string.Empty;
				return parentWrapper.Path;
			}
		}

		public string FirstChildElementPath {
			get {
				if (fragment == null)
					return string.Empty;
				var child = fragment.Navigate (NavigateDirection.FirstChild);
				if (child == null)
					return string.Empty;
				ProviderElementWrapper childWrapper =
					AutomationBridge.Instance.GetProviderWrapper (child);
				if (childWrapper == null)
					return string.Empty;
				return childWrapper.Path;
			}
		}

		public string LastChildElementPath {
			get {
				if (fragment == null)
					return string.Empty;
				var child = fragment.Navigate (NavigateDirection.LastChild);
				if (child == null)
					return string.Empty;
				ProviderElementWrapper childWrapper =
					AutomationBridge.Instance.GetProviderWrapper (child);
				if (childWrapper == null)
					return string.Empty;
				return childWrapper.Path;
			}
		}

		public string NextSiblingElementPath {
			get {
				if (fragment == null)
					return string.Empty;
				var sibling = fragment.Navigate (NavigateDirection.NextSibling);
				if (sibling == null)
					return string.Empty;
				ProviderElementWrapper siblingWrapper =
					AutomationBridge.Instance.GetProviderWrapper (sibling);
				if (siblingWrapper == null)
					return string.Empty;
				return siblingWrapper.Path;
			}
		}

		public string PreviousSiblingElementPath {
			get {
				if (fragment == null)
					return string.Empty;
				var sibling = fragment.Navigate (NavigateDirection.PreviousSibling);
				if (sibling == null)
					return string.Empty;
				ProviderElementWrapper siblingWrapper =
					AutomationBridge.Instance.GetProviderWrapper (sibling);
				if (siblingWrapper == null)
					return string.Empty;
				return siblingWrapper.Path;
			}
		}

		public string GetCurrentPatternPath (int patternId)
		{
			object patternProvider = provider.GetPatternProvider (patternId);
			if (patternProvider == null)
				return string.Empty;

			string patternPath;
			if (patternPathMapping.TryGetValue (patternId, out patternPath))
				return patternPath;

			patternPath = this.Path + "/";
			object patternObject = null;

			if (patternId == InvokePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.InvokePatternSubPath;
				patternObject = new InvokePatternWrapper ((IInvokeProvider) patternProvider);
			} else if (patternId == ValuePatternIdentifiers.Pattern.Id) {
				patternPath += DC.Constants.ValuePatternSubPath;
				patternObject = new ValuePatternWrapper ((IValueProvider) patternProvider);
			} else
				throw new InvalidOperationException ();

			if (bus == null)
				throw new ElementNotAvailableException ();

			bus.Register (new ObjectPath (patternPath), patternObject);
			patternPathMapping.Add(patternId, patternPath);
			return patternPath;
		}

#endregion

#region Public Methods and Properties

		public string Path {
			get { return DC.Constants.AutomationElementBasePath + pathId.ToString (); }
		}

		public void Register (Bus bus)
		{
			this.bus = bus;
			bus.Register (new ObjectPath (DC.Constants.AutomationElementBasePath + pathId.ToString ()),
			              this);
		}

		public void Unregister ()
		{
			bus.Unregister (new ObjectPath (DC.Constants.AutomationElementBasePath + pathId.ToString ()));
			foreach (string patternPath in patternPathMapping.Values)
				bus.Unregister (new ObjectPath (patternPath));
			patternPathMapping.Clear ();
		}

		public void UnregisterPattern (int patternId)
		{
			string patternPath;
			if (patternPathMapping.TryGetValue (patternId, out patternPath)) {
				patternPathMapping.Remove (patternId);
				bus.Unregister (new ObjectPath (patternPath));
			}
		}
#endregion
	}
}
