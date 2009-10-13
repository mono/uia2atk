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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
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
	public class UiaDbusTransformPattern : ITransformPattern
	{
		private DCI.ITransformPattern pattern;

		public UiaDbusTransformPattern (DCI.ITransformPattern pattern)
		{
			this.pattern = pattern;
		}

		public void Move (double x, double y)
		{
			try {
				pattern.Move (x, y);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void Resize (double w, double h)
		{
			try {
				pattern.Resize (w, h);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public void Rotate (double degrees)
		{
			try {
				pattern.Rotate (degrees);
			} catch (Exception ex) {
				throw DbusExceptionTranslator.Translate (ex);
			}
		}

		public TransformProperties Properties {
			get {

				try {
					TransformProperties properties = new TransformProperties () {
						CanMove = pattern.CanMove,
						CanResize = pattern.CanResize,
						CanRotate = pattern.CanRotate
					};
					return properties;
				} catch (Exception ex) {
					throw DbusExceptionTranslator.Translate (ex);
				}
			}
		}
	}
}