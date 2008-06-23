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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms
{
	public class StatusBarProvider : SimpleControlProvider, IGridProvider
	{
#region Private Members

        	private StatusBar statusBar;

#endregion

#region Constructors

        	public StatusBarProvider (StatusBar statusBar) : base (statusBar)
        	{
            		this.statusBar = statusBar;
        	}

#endregion

#region Public Methods

        	public override void InitializeEvents ()
        	{
            		base.InitializeEvents ();
        	}

#endregion

#region IRawElementProviderSimple Members

        	public override object GetPatternProvider (int patternId)
        	{
            		if (patternId == GridPatternIdentifiers.Pattern.Id)
                		return this;

			return null;
        	}

        	public override object GetPropertyValue (int propertyId)
        	{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.StatusBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "status bar";
			else if (propertyId == GridPatternIdentifiers.ColumnCountProperty.Id)
				return ColumnCount;
			else if (propertyId == GridPatternIdentifiers.RowCountProperty.Id)
				return RowCount;
			else
				return base.GetPropertyValue (propertyId);
        	}

#endregion

#region IGridProvider Members

        	public int ColumnCount {
			get { return 1; }
        	}

        	public int RowCount {
            		get { return 1; }
        	}

        	public IRawElementProviderSimple GetItem (int row, int column)
        	{
			if (column > ColumnCount || row > RowCount)
				throw new ArgumentOutOfRangeException ();
			if (column < 0 || row < 0)
				throw new ArgumentOutOfRangeException ();
			return null;
        	}

#endregion
    	}
}
