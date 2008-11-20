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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;

namespace Mono.UIAutomation.Winforms.Events
{

	enum ProviderEventType
	{
		//Automation Element Properties
		AutomationElementIsOffscreenProperty,
		AutomationElementIsEnabledProperty,
		AutomationElementNameProperty,
		AutomationElementHasKeyboardFocusProperty,
		AutomationElementBoundingRectangleProperty,
		AutomationElementLabeledByProperty,
		AutomationElementIsKeyboardFocusableProperty,
		AutomationElementControlTypeProperty,
		AutomationElementIsPatternAvailableProperty, //Used for each Is*PatternAvailable
		//Automation Events
		AutomationFocusChangedEvent,
		//Text Pattern
		TextPatternTextChangedEvent,
		TextPatternTextSelectionChangedEvent,
		//TODO>
		StructureChangedEvent,
		//Toggle Pattern
		TogglePatternToggleStateProperty,
		//Invoke Pattern
		InvokePatternInvokedEvent,
		//ExpandCollapse Pattern
		ExpandCollapsePatternExpandCollapseStateProperty,
		//Value Pattern
		ValuePatternValueProperty,
		ValuePatternIsReadOnlyProperty,
		//Selection Pattern
		SelectionPatternInvalidatedEvent,
		SelectionPatternCanSelectMultipleProperty,
		SelectionPatternIsSelectionRequiredProperty,
		SelectionPatternSelectionProperty,
		//SelectionItem Pattern
		SelectionItemPatternElementAddedEvent,
		SelectionItemPatternElementRemovedEvent,
		SelectionItemPatternElementSelectedEvent,
		SelectionItemPatternSelectionContainerProperty,
		SelectionItemPatternIsSelectedProperty,
		//RangeValue Pattern
		RangeValuePatternValueProperty,
		RangeValuePatternIsReadOnlyProperty,
		RangeValuePatternMinimumProperty,
		RangeValuePatternMaximumProperty,
		RangeValuePatternLargeChangeProperty,
		RangeValuePatternSmallChangeProperty,
		//Grid Pattern
		GridPatternRowCountProperty,
		GridPatternColumnCountProperty,
		//GridItem Pattern
		GridItemPatternRowProperty,
		GridItemPatternColumnProperty,
		GridItemPatternRowSpanProperty,
		GridItemPatternColumnSpanProperty,
		GridItemPatternContainingGridProperty,
		//Scroll Pattern
		ScrollPatternHorizontalViewSizeProperty,
		ScrollPatternVerticalViewSizeProperty,
		ScrollPatternHorizontalScrollPercentProperty,
		ScrollPatternVerticalScrollPercentProperty,		
		ScrollPatternVerticallyScrollableProperty,
		ScrollPatternHorizontallyScrollableProperty,
		//Table Pattern
		TablePatternColumnHeadersProperty,
		TablePatternRowHeadersProperty,
		TablePatternRowOrColumnMajorProperty,
		//Table Item Pattern
		TableItemPatternColumnHeaderItemsProperty,
		TableItemPatternRowHeaderItemsProperty,
		//Transform Pattern
		TransformPatternCanMoveProperty,
		TransformPatternCanResizeProperty,
		TransformPatternCanRotateProperty,
		//Dock Pattern
		DockPatternDockPositionProperty
	}
}
