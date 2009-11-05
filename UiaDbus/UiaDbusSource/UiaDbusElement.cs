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
			try {
				return dbusElement.SupportsProperty (property.Id);
			} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public string AcceleratorKey {
			get {
				try {
					return dbusElement.AcceleratorKey;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string AccessKey {
			get {
				try {
					return dbusElement.AccessKey;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
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
				DC.Rect dbusRect;
				try {
					dbusRect = dbusElement.BoundingRectangle;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return dbusRect.ToSWRect ();
			}
		}

		public string ClassName {
			get {
				try {
					return dbusElement.ClassName;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public Point ClickablePoint {
			get {
				DC.Point dbusPoint;
				try {
					dbusPoint = dbusElement.ClickablePoint;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return new Point (dbusPoint.x, dbusPoint.y);
			}
		}

		public ControlType ControlType {
			get {
				try {
					return ControlType.LookupById (dbusElement.ControlTypeId);
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string FrameworkId {
			get {
				try {
					return dbusElement.FrameworkId;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool HasKeyboardFocus {
			get {
				try {
					return dbusElement.HasKeyboardFocus;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string HelpText {
			get {
				try {
					return dbusElement.HelpText;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsContentElement {
			get {
				try {
					return dbusElement.IsContentElement;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsControlElement {
			get {
				try {
					return dbusElement.IsControlElement;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsEnabled {
			get {
				try {
					return dbusElement.IsEnabled;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsKeyboardFocusable {
			get {
				try {
					return dbusElement.IsKeyboardFocusable;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsOffscreen {
			get {
				try {
					return dbusElement.IsOffscreen;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsPassword {
			get {
				try {
					return dbusElement.IsPassword;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public bool IsRequiredForForm {
			get {
				try {
					return dbusElement.IsRequiredForForm;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string ItemStatus {
			get {
				try {
					return dbusElement.ItemStatus;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string ItemType {
			get {
				try {
					return dbusElement.ItemType;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public IElement LabeledBy {
			get {
				string labeledByPath = null;
				try {
					labeledByPath = dbusElement.LabeledByElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				IElement labeledBy = null;
				if (!string.IsNullOrEmpty (labeledByPath))
					labeledBy = source.GetOrCreateElement (busName, labeledByPath);
				return labeledBy;
			}
		}

		public string LocalizedControlType {
			get {
				try {
					return dbusElement.LocalizedControlType;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public string Name {
			get {
				try {
					return dbusElement.Name;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public int NativeWindowHandle {
			get {
				try {
					return dbusElement.NativeWindowHandle;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public OrientationType Orientation {
			get {
				try {
					return dbusElement.Orientation;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public int ProcessId {
			get {
				try {
					return dbusElement.ProcessId;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public int [] RuntimeId {
			get {
				try {
					return dbusElement.RuntimeId;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public IElement Parent {
			get {
				string path = null;
				try {
					path = dbusElement.ParentElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				if (string.IsNullOrEmpty (path))
					return null;
				return source.GetOrCreateElement (busName, path);
			}
		}

		public IElement FirstChild {
			get {
				string firstChildPath = null;
				try {
					firstChildPath = dbusElement.FirstChildElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return source.GetOrCreateElement (busName, firstChildPath);
			}
		}

		public IElement LastChild {
			get {
				string lastChildPath = null;
				try {
					lastChildPath = dbusElement.LastChildElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return source.GetOrCreateElement (busName, lastChildPath);
			}
		}

		public IElement NextSibling {
			get {
				string nextSiblingPath = null;
				try {
					nextSiblingPath = dbusElement.NextSiblingElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return source.GetOrCreateElement (busName, nextSiblingPath);
			}
		}

		public IElement PreviousSibling {
			get {
				string previousSiblingPath = null;
				try {
					previousSiblingPath = dbusElement.PreviousSiblingElementPath;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
				return source.GetOrCreateElement (busName, previousSiblingPath);
			}
		}

		public object GetCurrentPattern (AutomationPattern pattern)
		{
			if (pattern == null)
				throw new InvalidOperationException ();
			string patternPath = null;
			try {
				patternPath = dbusElement.GetCurrentPatternPath (pattern.Id);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
			if (string.IsNullOrEmpty (patternPath))
				throw new InvalidOperationException ();
			object ret = null;

			if (pattern.Id == DockPatternIdentifiers.Pattern.Id) {
				DCI.IDockPattern dockPattern = Bus.Session.GetObject<DCI.IDockPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.DockPatternSubPath));
				ret = new UiaDbusDockPattern (dockPattern);
			} else if (pattern.Id == ExpandCollapsePatternIdentifiers.Pattern.Id) {
				DCI.IExpandCollapsePattern expandCollapsePattern = Bus.Session.GetObject<DCI.IExpandCollapsePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.ExpandCollapsePatternSubPath));
				ret = new UiaDbusExpandCollapsePattern (expandCollapsePattern);
			} else if (pattern.Id == GridPatternIdentifiers.Pattern.Id) {
				DCI.IGridPattern gridPattern = Bus.Session.GetObject<DCI.IGridPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.GridPatternSubPath));
				ret = new UiaDbusGridPattern (gridPattern, busName, source);
			} else if (pattern.Id == GridItemPatternIdentifiers.Pattern.Id) {
				DCI.IGridItemPattern gridItemPattern = Bus.Session.GetObject<DCI.IGridItemPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.GridItemPatternSubPath));
				ret = new UiaDbusGridItemPattern (gridItemPattern, busName, source);
			} else if (pattern.Id == InvokePatternIdentifiers.Pattern.Id) {
				DCI.IInvokePattern invokePattern = Bus.Session.GetObject<DCI.IInvokePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.InvokePatternSubPath));
				ret = new UiaDbusInvokePattern (invokePattern);
			} else if (pattern.Id == MultipleViewPatternIdentifiers.Pattern.Id) {
				DCI.IMultipleViewPattern multipleViewPattern = Bus.Session.GetObject<DCI.IMultipleViewPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.MultipleViewPatternSubPath));
				ret = new UiaDbusMultipleViewPattern (multipleViewPattern);
			} else if (pattern.Id == RangeValuePatternIdentifiers.Pattern.Id) {
				DCI.IRangeValuePattern rangeValuePattern = Bus.Session.GetObject<DCI.IRangeValuePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.RangeValuePatternSubPath));
				ret = new UiaDbusRangeValuePattern (rangeValuePattern);
			} else if (pattern.Id == ScrollItemPatternIdentifiers.Pattern.Id) {
				DCI.IScrollItemPattern scrollItemPattern = Bus.Session.GetObject<DCI.IScrollItemPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.ScrollItemPatternSubPath));
				ret = new UiaDbusScrollItemPattern (scrollItemPattern);
			} else if (pattern.Id == ScrollPatternIdentifiers.Pattern.Id) {
				DCI.IScrollPattern scrollPattern = Bus.Session.GetObject<DCI.IScrollPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.ScrollPatternSubPath));
				ret = new UiaDbusScrollPattern (scrollPattern);
			} else if (pattern.Id == SelectionPatternIdentifiers.Pattern.Id) {
				DCI.ISelectionPattern selectionPattern = Bus.Session.GetObject<DCI.ISelectionPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.SelectionPatternSubPath));
				ret = new UiaDbusSelectionPattern (selectionPattern, busName, source);
			} else if (pattern.Id == SelectionItemPatternIdentifiers.Pattern.Id) {
				DCI.ISelectionItemPattern selectionItemPattern = Bus.Session.GetObject<DCI.ISelectionItemPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.SelectionItemPatternSubPath));
				ret = new UiaDbusSelectionItemPattern (selectionItemPattern, busName, source);
			} else if (pattern.Id == TablePatternIdentifiers.Pattern.Id) {
				DCI.ITablePattern tablePattern = Bus.Session.GetObject<DCI.ITablePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TablePatternSubPath));
				ret = new UiaDbusTablePattern (tablePattern, busName, source);
			} else if (pattern.Id == TableItemPatternIdentifiers.Pattern.Id) {
				DCI.ITableItemPattern tableItemPattern = Bus.Session.GetObject<DCI.ITableItemPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TableItemPatternSubPath));
				ret = new UiaDbusTableItemPattern (tableItemPattern, busName, source);
			} else if (pattern.Id == TextPatternIdentifiers.Pattern.Id) {
				DCI.ITextPattern textPattern = Bus.Session.GetObject<DCI.ITextPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TextPatternSubPath));
				ret = new UiaDbusTextPattern (textPattern, busName, source);
			} else if (pattern.Id == TogglePatternIdentifiers.Pattern.Id) {
				DCI.ITogglePattern togglePattern = Bus.Session.GetObject<DCI.ITogglePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TogglePatternSubPath));
				ret = new UiaDbusTogglePattern (togglePattern);
			} else if (pattern.Id == ValuePatternIdentifiers.Pattern.Id) {
				DCI.IValuePattern valuePattern = Bus.Session.GetObject<DCI.IValuePattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.ValuePatternSubPath));
				ret = new UiaDbusValuePattern (valuePattern);
			} else if (pattern.Id == TransformPatternIdentifiers.Pattern.Id) {
				DCI.ITransformPattern transformPattern = Bus.Session.GetObject<DCI.ITransformPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.TransformPatternSubPath));
				ret = new UiaDbusTransformPattern (transformPattern);
			} else if (pattern.Id == WindowPatternIdentifiers.Pattern.Id) {
				DCI.IWindowPattern windowPattern = Bus.Session.GetObject<DCI.IWindowPattern>
					(busName, new ObjectPath (dbusPath + "/" + DC.Constants.WindowPatternSubPath));
				ret = new UiaDbusWindowPattern (windowPattern);
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
