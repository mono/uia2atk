using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace CustomControl
{
	public class UselessChildControlPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
	{
		public UselessChildControlPeer (UselessChildControl owner)
			: base (owner)
		{
			childControl = owner;
			childControl.Selected += delegate (object sender, EventArgs e) {
				RaiseSelectionItemAutomationEvent ();
			};

			Grid grid = (Grid) owner.Parent;
			parentPeer = FrameworkElementAutomationPeer.CreatePeerForElement ((UIElement) grid.Parent);
		}

		public override object GetPattern (PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.SelectionItem)
				return this;

			return base.GetPattern(patternInterface);
		}

		#region ISelectionItemProvider Members

		void ISelectionItemProvider.AddToSelection ()
		{
			((ISelectionItemProvider)this).Select ();
		}

		bool ISelectionItemProvider.IsSelected
		{
			get { return childControl.IsSelected; }
		}

		void ISelectionItemProvider.RemoveFromSelection ()
		{
			if (childControl.IsSelected)
				childControl.IsSelected = false;
		}

		void ISelectionItemProvider.Select ()
		{
			if (!childControl.IsSelected)
				childControl.IsSelected = true;
		}

		IRawElementProviderSimple ISelectionItemProvider.SelectionContainer {
			get { return ProviderFromPeer (parentPeer); }
		}

		#endregion

		void RaiseSelectionItemAutomationEvent ()
		{
			ISelectionProvider selectionProvider = parentPeer.GetPattern(PatternInterface.Selection) as ISelectionProvider; 
			if (selectionProvider == null)
			    return;

			IRawElementProviderSimple[] selected = selectionProvider.GetSelection ();
			if (childControl.IsSelected) {
				if (selected.Length == 1)
					RaiseAutomationEvent (AutomationEvents.SelectionItemPatternOnElementSelected);
				else
					RaiseAutomationEvent (AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
			} else
				RaiseAutomationEvent (AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
		}

		AutomationPeer parentPeer;
		UselessChildControl childControl;
	}
}
