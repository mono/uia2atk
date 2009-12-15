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
	public class UiaDbusTablePattern : UiaDbusGridPattern, ITablePattern
	{
		private DCI.ITablePattern pattern;
		private string busName;
		private UiaDbusAutomationSource source;

		public UiaDbusTablePattern (DCI.ITablePattern pattern, string busName,
		                           UiaDbusAutomationSource source)
			: base (DCI.InterfaceConverter.Table2Grid (pattern),
			        busName, source)
		{
			this.pattern = pattern;
			this.busName = busName;
			this.source = source;
		}

		public RowOrColumnMajor RowOrColumnMajor {
			get {
				try {
					return pattern.RowOrColumnMajor;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}

		public IElement [] GetRowHeaders ()
		{
			try {
				return source.GetOrCreateElements (busName, pattern.GetRowHeaderPaths ());
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public IElement [] GetColumnHeaders ()
		{
			try {
				return source.GetOrCreateElements (busName, pattern.GetColumnHeaderPaths ());
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}
	}
}
