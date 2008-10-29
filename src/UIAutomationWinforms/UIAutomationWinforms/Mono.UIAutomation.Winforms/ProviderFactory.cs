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
using System.Collections.Generic;
using System.ComponentModel;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public static class ProviderFactory
	{	
		#region Static Fields
		
		// NOTE: This may not be the best place to track this...however
		//       I forsee this factory class evolving into a builder
		//       class that takes raw providers and attaches provider
		//       behaviors depending on the control type, and maybe
		//       it makes sense for the builder to keep track of
		//       this mapping?
		private static Dictionary<Component, IRawElementProviderFragment>
			componentProviders;
		
		private static List<IRawElementProviderFragmentRoot> formProviders;
		
		static ProviderFactory ()
		{
			componentProviders =
				new Dictionary<Component,IRawElementProviderFragment> ();
			
			formProviders = new List<IRawElementProviderFragmentRoot> ();
		}
		
		#endregion
		
		#region Static Public Methods
		
		public static List<IRawElementProviderFragmentRoot> GetFormProviders () 
		{
			return formProviders;
		}
		
		public static IRawElementProviderFragment GetProvider (Component component)
		{
			return GetProvider (component, true, false);
		}
		
		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize)
		{
			return GetProvider (component, initialize, false);
		}

		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize,
		                                                       bool forceInitializeChildren)
		{
			SWF.Label l;
			SWF.Button b;
			SWF.RadioButton r;
			SWF.CheckBox c;
			SWF.TextBox t;
			SWF.RichTextBox rt;
			SWF.LinkLabel ll;
			SWF.UpDownBase udb;
			FragmentControlProvider provider = null;
			SWF.Form f;
			SWF.GroupBox gb;
			SWF.StatusBar sb;
			SWF.ComboBox cb;
			SWF.CheckedListBox clb;
			SWF.ListBox lb;
			SWF.ScrollBar scb;
			SWF.PictureBox pb;
			SWF.ToolTip tt;
			SWF.ProgressBar pgb;
			SWF.HelpProvider hlp;
			SWF.ErrorProvider errp;
			SWF.TabControl tc;
			SWF.TabPage tp;
//			SWF.ListView lv;
			SWF.WebBrowser wb;
			SWF.Panel p;
			
			SWF.MenuStrip ms;
			SWF.ToolStrip ts;
			SWF.ToolStripMenuItem tsmi;
			SWF.ToolStripLabel tsl;
			SWF.ToolStripItem tsi;
			SWF.ToolStripTextBox tstb;
			
			if (component == null)
				return null;

			provider = (FragmentControlProvider) FindProvider (component);
			if (provider != null)
				return provider;

			if ((f = component as SWF.Form) != null) {
				provider = new FormProvider (f);
				formProviders.Add ((IRawElementProviderFragmentRoot) provider);
			} else if ((wb = component as SWF.WebBrowser) != null)
				provider = new WebBrowserProvider (wb);
			else if ((gb = component as SWF.GroupBox) != null)
				provider = new GroupBoxProvider (gb);
			else if ((b = component as SWF.Button) != null)
				provider = new ButtonProvider (b);
			else if ((r = component as SWF.RadioButton) != null)
				provider = new RadioButtonProvider (r);
			else if ((c = component as SWF.CheckBox) != null)
				provider = new CheckBoxProvider (c);
			else if ((t = component as SWF.TextBox) != null)
				provider = new TextBoxProvider (t);
			else if ((ll = component as SWF.LinkLabel) != null)
				provider = new LinkLabelProvider (ll);
			else if ((l = component as SWF.Label) != null)
				provider = new LabelProvider (l);
			else if ((udb = component as SWF.UpDownBase) != null)
				provider = new UpDownBaseProvider (udb);
			else if ((sb = component as SWF.StatusBar) != null)
				provider = new StatusBarProvider (sb);
			else if ((cb = component as SWF.ComboBox) != null)
				provider = new ComboBoxProvider (cb);
			else if ((clb = component as SWF.CheckedListBox) != null)
				provider = new CheckedListBoxProvider (clb);
			else if ((lb = component as SWF.ListBox) != null)
				provider = new ListBoxProvider (lb);
			else if ((pgb = component as SWF.ProgressBar) != null)
				provider = new ProgressBarProvider (pgb);
			else if ((ms = component as SWF.MenuStrip) != null)
				provider = new MenuStripProvider (ms);
			else if ((ts = component as SWF.ToolStrip) != null)
				provider = new ToolStripProvider (ts);
			else if ((tsmi = component as SWF.ToolStripMenuItem) != null)
				provider = new ToolStripMenuItemProvider (tsmi);
			else if ((tsl = component as SWF.ToolStripLabel) != null)
				provider = new ToolStripLabelProvider (tsl);
			else if ((tstb = component as SWF.ToolStripTextBox) != null)
				provider = new ToolStripTextBoxProvider (tstb);
			else if ((tsi = component as SWF.ToolStripItem) != null)
				provider = new ToolStripItemProvider (tsi);
			else if ((scb = component as SWF.ScrollBar) != null) {
				//TODO:
				//   We need to add here a ScrollableControlProvider and then verify
				//   if the internal scrollbar instances are matching this one,
				//   if so, then we return a scrollbar, otherwise we return a pane.
#pragma warning disable 219
				SWF.ScrollableControl scrollable;
				//ScrollableControlProvider scrollableProvider;
				if ((scrollable = scb.Parent as SWF.ScrollableControl) != null
				    || scb.Parent == null) {
#pragma warning restore 219
				//	scrollableProvider = (ScrollableControlProvider) GetProvider (scrollable);
				//	if (scrollableProvider.ScrollBarExists (scb) == true)
						provider = new ScrollBarProvider (scb);
				//	else 
				//		provider = new PaneProvider (scb);
				} else
					provider = new PaneProvider (scb);
			} else if ((pb = component as SWF.PictureBox) != null)
				provider = new PictureBoxProvider (pb);
			else if ((errp = component as SWF.ErrorProvider) != null)
				provider = new ErrorProvider (errp);
			else if ((tt = component as SWF.ToolTip) != null)
				provider = new ToolTipProvider (tt);
			else if ((hlp = component as SWF.HelpProvider) != null)
				provider = new HelpProvider (hlp);
			else if ((tc = component as SWF.TabControl) != null)
				provider = new TabControlProvider (tc);
			else if ((tp = component as SWF.TabPage) != null)
				provider = new TabPageProvider (tp);
			else if ((rt = component as SWF.RichTextBox) != null)
				provider = new RichTextBoxProvider (rt);
			else if ((p = component as SWF.Panel) != null)
				provider = new PanelProvider (p);
//			else if ((lv = component as SWF.ListView) != null)
//				provider = new ListViewProvider (lv);
			else {
				//TODO: We have to solve the problem when there's a Custom control
				//	Ideally the first thing we do is send a wndproc message to
				//	the control to see if it implements its own provider.
				//	After all, the control may derive from a base control in SWF.
				//	So that check comes before this big if/else block.
				
				//FIXME: let's not throw while we are developing, a big WARNING will suffice
				//throw new NotImplementedException ("Provider not implemented for control " + component.GetType().Name);
				Console.WriteLine ("WARNING: Provider not implemented for control " + component.GetType().Name);
				return null;
			}
			
			if (provider != null) {
				// TODO: Make tracking in dictionary optional
				componentProviders [component] = provider;
				if (initialize)
					provider.Initialize ();
				
				FragmentRootControlProvider root;
				if (forceInitializeChildren == true
				    && (root = provider as FragmentRootControlProvider) != null)
					root.InitializeChildControlStructure ();
			}
			
			return provider;
		}

		public static void ReleaseProvider (Component component)
		{
			IRawElementProviderFragment provider;
			if (componentProviders.TryGetValue (component, 
			                                    out provider) == true) {
				componentProviders.Remove (component);
				((FragmentControlProvider) provider).Terminate ();
				if (provider is FormProvider)
					formProviders.Remove ((FormProvider) provider);
			}
		}
		
		public static IRawElementProviderFragment FindProvider (Component component)
		{
			IRawElementProviderFragment provider;
			
			if (component == null)
				return null;
			else if (componentProviders.TryGetValue (component, out provider))
				return provider;

			return null;
		}
		
		#endregion
	}
}
