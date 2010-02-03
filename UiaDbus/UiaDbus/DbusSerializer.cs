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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Windows.Automation;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using SW = System.Windows;

namespace Mono.UIAutomation.UiaDbus
{
	public static class DbusSerializer
	{
		private const string SpecialValueHeader = "__uia_special";

		public static object SerializeValue (int propId, object value)
		{
			if (value == null)
				return new string [] { SpecialValueHeader, "null" };
			else if (propId == AEIds.BoundingRectangleProperty.Id) {
				var rect = (SW.Rect) value;
				return new double [] {
					rect.Left,
					rect.Top,
					rect.Width,
					rect.Height
				};
			} else if (propId == AEIds.ClickablePointProperty.Id) {
				var point = (SW.Point) value;
				return new double [] {
					point.X,
					point.Y,
				};
			} else
				return value;
		}

		public static object DeserializeValue (int propId, object value)
		{
			string [] special = value as string [];
			if (special != null && special.Length == 2 && special[0] == SpecialValueHeader) {
				if (special[1] == "null")
					return null;
				else
					return value;
			} else if (propId == AEIds.BoundingRectangleProperty.Id) {
				var data = (double []) value;
				var rect = new SW.Rect (data [0], data [1], data [2], data [3]);
				if (rect.IsEmpty)
					rect = SW.Rect.Empty;
				return rect;
			} else if (propId == AEIds.ClickablePointProperty.Id) {
				var data = (double []) value;
				return new SW.Point (data [0], data [1]);
			} else
				return value;
		}
	}
}
