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
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using DC = Mono.UIAutomation.UiaDbus;
using DCI = Mono.UIAutomation.UiaDbus.Interfaces;

namespace Mono.UIAutomation.UiaDbusSource
{
	public class UiaDbusScrollPattern : IScrollPattern
	{
		private DCI.IScrollPattern pattern;

		public UiaDbusScrollPattern (DCI.IScrollPattern pattern)
		{
			this.pattern = pattern;
		}

		public ScrollProperties Properties {
			get {
				try {
					ScrollProperties properties = new ScrollProperties () {
						HorizontallyScrollable = pattern.HorizontallyScrollable,
						HorizontalScrollPercent = pattern.HorizontalScrollPercent,
						HorizontalViewSize = pattern.HorizontalViewSize,
						VerticallyScrollable = pattern.VerticallyScrollable,
						VerticalScrollPercent = pattern.VerticalScrollPercent,
						VerticalViewSize = pattern.VerticalViewSize
					};
					return properties;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			try {
				pattern.Scroll (horizontalAmount, verticalAmount);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			try {
				pattern.SetScrollPercent (horizontalPercent, verticalPercent);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}
	}
}
