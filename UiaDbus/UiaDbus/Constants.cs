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

namespace Mono.UIAutomation.UiaDbus
{
	public static class Constants
	{
		public const string ApplicationPath = "/org/mono/UIAutomation/Application";
		public const string Namespace = "org.mono.UIAutomation";
		public const string AutomationElementInterfaceName = Namespace + ".AutomationElement";
		public const string ApplicationInterfaceName = Namespace + ".Application";
		public const string AutomationElementBasePath = "/org/mono/UIAutomation/Element/";

		public const string DockPatternInterfaceName = Namespace + ".DockPattern";
		public const string ExpandCollapsePatternInterfaceName = Namespace + ".ExpandCollapsePattern";
		public const string GridItemPatternInterfaceName = Namespace + ".GridItemPattern";
		public const string GridPatternInterfaceName = Namespace + ".GridPattern";
		public const string InvokePatternInterfaceName = Namespace + ".InvokePattern";
		public const string MultipleViewPatternInterfaceName = Namespace + ".MultipleViewPattern";
		public const string RangeValuePatternInterfaceName = Namespace + ".RangeValuePattern";
		public const string ScrollItemPatternInterfaceName = Namespace + ".ScrollItemPattern";
		public const string ScrollPatternInterfaceName = Namespace + ".ScrollPattern";
		public const string SelectionItemPatternInterfaceName = Namespace + ".SelectionItemPattern";
		public const string SelectionPatternInterfaceName = Namespace + ".SelectionPattern";
		public const string TextPatternInterfaceName = Namespace + ".TextPattern";
		public const string TogglePatternInterfaceName = Namespace + ".TogglePattern";
		public const string TransformPatternInterfaceName = Namespace + ".TransformPattern";
		public const string ValuePatternInterfaceName = Namespace + ".ValuePattern";
		public const string WindowPatternInterfaceName = Namespace + ".WindowPattern";

		public const string DockPatternSubPath = "Dock";
		public const string ExpandCollapsePatternSubPath = "ExpandCollapse";
		public const string GridItemPatternSubPath = "GridItem";
		public const string GridPatternSubPath = "Grid";
		public const string InvokePatternSubPath = "Invoke";
		public const string MultipleViewPatternSubPath = "MultipleView";
		public const string RangeValuePatternSubPath = "RangeValue";
		public const string ScrollItemPatternSubPath = "ScrollItem";
		public const string ScrollPatternSubPath = "Scroll";
		public const string SelectionItemPatternSubPath = "SelectionItem";
		public const string SelectionPatternSubPath = "Selection";
		public const string TextPatternSubPath = "Text";
		public const string TogglePatternSubPath = "Toggle";
		public const string TransformPatternSubPath = "Transform";
		public const string ValuePatternSubPath = "Value";
		public const string WindowPatternSubPath = "Window";

	}
}
