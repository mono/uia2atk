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
using System.Windows;
using System.Windows.Automation;

using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using DC = Mono.UIAutomation.UiaDbus;
using DCI = Mono.UIAutomation.UiaDbus.Interfaces;

using NDesk.DBus;

namespace Mono.UIAutomation.UiaDbusSource
{
	public class UiaDbusElement : IElement
	{
		#region Private FIelds

		private DCI.IAutomationElement dbusElement;
		private UiaDbusAutomationSource source;
		private string busName;
		private string dbusPath;

		#endregion

		#region Constructor

		public UiaDbusElement (DCI.IAutomationElement dbusElement, string busName,
		                        string dbusPath, UiaDbusAutomationSource source)
		{
			this.source = source;
			this.busName = busName;
			this.dbusPath = dbusPath;
			this.dbusElement = dbusElement;
		}

		#endregion

		#region IElement implementation

		public bool SupportsProperty (AutomationProperty property)
		{
			return dbusElement.SupportsProperty (property.Id);
		}

		public string AcceleratorKey {
			get {
				return dbusElement.AcceleratorKey;
			}
		}

		public string AccessKey {
			get {
				return dbusElement.AccessKey;
			}
		}

		public string AutomationId {
			get {
				try {
					return dbusElement.AutomationId;
				} catch {
					return string.Empty;
				}
			}
		}

		public Rect BoundingRectangle {
			get {
				DC.Rect dbusRect = dbusElement.BoundingRectangle;
				return dbusRect.ToSWRect ();
			}
		}

		public string ClassName {
			get {
				return dbusElement.ClassName;
			}
		}

		public Point ClickablePoint {
			get {
				DC.Point dbusPoint = dbusElement.ClickablePoint;
				return new Point (dbusPoint.x, dbusPoint.y);
			}
		}

		public ControlType ControlType {
			get {
				return ControlType.LookupById (dbusElement.ControlTypeId);
			}
		}

		public string FrameworkId {
			get {
				return dbusElement.FrameworkId;
			}
		}

		public bool HasKeyboardFocus {
			get {
				return dbusElement.HasKeyboardFocus;
			}
		}

		public string HelpText {
			get {
				return dbusElement.HelpText;
			}
		}

		public bool IsContentElement {
			get {
				return dbusElement.IsContentElement;
			}
		}

		public bool IsControlElement {
			get {
				return dbusElement.IsControlElement;
			}
		}

		public bool IsEnabled {
			get {
				return dbusElement.IsEnabled;
			}
		}

		public bool IsKeyboardFocusable {
			get {
				return dbusElement.IsKeyboardFocusable;
			}
		}

		public bool IsOffscreen {
			get {
				return dbusElement.IsOffscreen;
			}
		}

		public bool IsPassword {
			get {
				return dbusElement.IsPassword;
			}
		}

		public bool IsRequiredForForm {
			get {
				return dbusElement.IsRequiredForForm;
			}
		}

		public string ItemStatus {
			get {
				return dbusElement.ItemStatus;
			}
		}

		public string ItemType {
			get {
				return dbusElement.ItemType;
			}
		}

		public IElement LabeledBy {
			get {
				IElement labeledBy = null;
				string labeledByPath = dbusElement.LabeledByElementPath;
				//Log.Debug ("UiaDbusElement.LabeledBy: " + labeledByPath + " AID: " + AutomationId + " CT: " + this.ControlType.ProgrammaticName);
				if (!string.IsNullOrEmpty (labeledByPath))
					labeledBy = source.GetOrCreateElement (busName, labeledByPath);
				return labeledBy;
			}
		}

		public string LocalizedControlType {
			get {
				return dbusElement.LocalizedControlType;
			}
		}

		public string Name {
			get {
				return dbusElement.Name;
			}
		}

		public int NativeWindowHandle {
			get {
				return dbusElement.NativeWindowHandle;
			}
		}

		public OrientationType Orientation {
			get {
				return dbusElement.Orientation;
			}
		}

		public int ProcessId {
			get {
				return dbusElement.ProcessId;
			}
		}

		public int [] RuntimeId {
			get {
				return dbusElement.RuntimeId;
			}
		}

		public IElement Parent {
			get {
				string path = dbusElement.ParentElementPath;
				if (string.IsNullOrEmpty (path))
					return null;
				return source.GetOrCreateElement (busName, path);
			}
		}

		public IElement FirstChild {
			get {
				return source.GetOrCreateElement (busName,
				                                  dbusElement.FirstChildElementPath);
			}
		}

		public IElement LastChild {
			get {
				return source.GetOrCreateElement (busName,
				                                  dbusElement.LastChildElementPath);
			}
		}

		public IElement NextSibling {
			get {
				return source.GetOrCreateElement (busName,
				                                  dbusElement.NextSiblingElementPath);
			}
		}

		public IElement PreviousSibling {
			get {
				return source.GetOrCreateElement (busName,
				                                  dbusElement.PreviousSiblingElementPath);
			}
		}

		public object GetCurrentPattern (AutomationPattern pattern)
		{
			if (pattern == null)
				throw new InvalidOperationException ();
			string patternPath = dbusElement.GetCurrentPatternPath (pattern.Id);
			if (string.IsNullOrEmpty (patternPath))
				throw new InvalidOperationException ();
			object ret = null;

			if (pattern.Id == InvokePatternIdentifiers.Pattern.Id) {
				DCI.IInvokePattern invokePattern = Bus.Session.GetObject<DCI.IInvokePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.InvokePatternSubPath));
				ret = new UiaDbusInvokePattern (invokePattern);
			} else if (pattern.Id == TextPatternIdentifiers.Pattern.Id) {
				DCI.ITextPattern textPattern = Bus.Session.GetObject<DCI.ITextPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TextPatternSubPath));
				ret = new UiaDbusTextPattern (textPattern, busName, source);
			} else if (pattern.Id == ValuePatternIdentifiers.Pattern.Id) {
				DCI.IValuePattern valuePattern = Bus.Session.GetObject<DCI.IValuePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.ValuePatternSubPath));
				ret = new UiaDbusValuePattern (valuePattern);
			} else if (pattern.Id == TransformPatternIdentifiers.Pattern.Id) {
				DCI.ITransformPattern transformPattern = Bus.Session.GetObject<DCI.ITransformPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TransformPatternSubPath));
				ret = new UiaDbusTransformPattern (transformPattern);
			} else
				throw new InvalidOperationException ();

			return ret;
		}

		public AutomationPattern [] GetSupportedPatterns ()
		{
			// TODO: Implement
			throw new NotSupportedException ();
		}
		#endregion

		#region Public Properties

		public string BusName
		{
			get {
				return busName;
			}
		}

		public string DbusPath {
			get {
				return dbusPath;
			}
		}

		#endregion
	}
}
