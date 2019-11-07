// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Copyright (c) 2019 AxxonSoft (http://axxonsoft.com)
//
// Authors:
//   Nikita Voronchev <nikita.voronchev@ru.axxonsoft.com>
//

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Behaviors.PropertyGrid
{
	internal class GridItemLegacyIAccessibleProviderBehavior
		: ProviderBehavior, ILegacyIAccessibleProvider
	{
#region Constructors
		public GridItemLegacyIAccessibleProviderBehavior (PropertyGridListItemProvider provider)
			: base (provider)
		{
		}
#endregion

#region ProviderBehavior Specialization
		public override AutomationPattern ProviderPattern {
			get { return LegacyIAccessiblePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
		}

		public override void Disconnect ()
		{
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == LegacyIAccessiblePatternIdentifiers.ChildIdProperty.Id)
				return ChildId;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.DefaultActionProperty.Id)
				return DefaultAction;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.DescriptionProperty.Id)
				return Description;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.HelpProperty.Id)
				return Help;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.KeyboardShortcutProperty.Id)
				return KeyboardShortcut;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.NameProperty.Id)
				return Name;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.RoleProperty.Id)
				return Role;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.StateProperty.Id)
				return State;
			else if (propertyId == LegacyIAccessiblePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}
#endregion

#region ILegacyIAccessibleProvider Specialization
		public int ChildId {
			get { return 0; }
		}

		public string DefaultAction {
			get { return ""; }
		}

		public string Description {
			get { return ""; }
		}

		public string Help {
			get { return ""; }
		}

		public string KeyboardShortcut {
			get { return ""; }
		}

		public string Name {
			get { return GridItemProvider.Name; }
		}

		public int Role {
			get { return (int)SWF.AccessibleRole.Row; }
		}

		public int State {
			get {
				var state = SWF.AccessibleStates.Selectable;

				if (GridItemProvider.IsReadOnly)
					state |= SWF.AccessibleStates.ReadOnly;

				if (GridItemProvider.PropertyGridViewProvider.IsItemSelected (GridItemProvider))
					state |= SWF.AccessibleStates.Selected;

				if (GridEntry.Expandable) {
					if (GridEntry.Expanded)
						state |= SWF.AccessibleStates.Expanded;
					else
						state |= SWF.AccessibleStates.Collapsed;
				}

				return (int)state;
			}
		}

		public string Value {
			get { return GridItemProvider.Value; }
		}

		public void DoDefaultAction ()
		{
			if (!GridEntry.Expandable)
				return;
			GridItemProvider.PropertyGridViewProvider.SelectItem (GridItemProvider);
			GridEntry.Expanded = !GridEntry.Expanded;
		}
#endregion

#region Private properties
		private PropertyGridListItemProvider GridItemProvider
		{
			get { return (PropertyGridListItemProvider) Provider; }
		}

		private SWF.PropertyGridInternal.GridEntry GridEntry
		{
			get { return GridItemProvider.Entry; }
		}
#endregion
	}
}
