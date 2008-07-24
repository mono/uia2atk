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
using System.Windows.Forms;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public sealed class ProviderFactory
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
		//Read comment in ReleaseProvider method for detailed information.
		private static Dictionary<Component, int> sharedComponents;
		
		static ProviderFactory ()
		{
			componentProviders =
				new Dictionary<Component,IRawElementProviderFragment> ();
			sharedComponents = new Dictionary<Component, int> ();
		}
		
		#endregion
		
		#region Static Public Methods
		
		public static IRawElementProviderFragment GetProvider (Component component)
		{
			return GetProvider (component, true);
		}
		
		public static IRawElementProviderFragment GetProvider (Component component, 
		                                                       bool initializeEvents)
		{
			return GetProvider (component, initializeEvents, true);
		}
		
		
		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initializeEvents,
		                                                       bool initializeChildControlStructure)
		{
			Label l;
			Button b;
			RadioButton r;
			CheckBox c;
			TextBox t;
			LinkLabel ll;
			FragmentControlProvider provider = null;
			Form f;
			GroupBox gb;
			StatusBar sb;
			ComboBox cb;
			ListBox lb;
			ScrollBar scb;
			PictureBox pb;
			ErrorProvider ep;
			ToolTip tt;
			bool isComponentBased = false;
			
			if (component == null)
				return null;

			provider = (FragmentControlProvider) FindProvider (component);
			if (provider != null)
				return provider;

			if ((f = component as Form) != null)
				provider = new WindowProvider (f);
			else if ((gb = component as GroupBox) != null)
				provider = new GroupBoxProvider (gb);
			else if ((b = component as Button) != null)
				provider = new ButtonProvider (b);
			else if ((r = component as RadioButton) != null)
				provider = new RadioButtonProvider (r);
			else if ((c = component as CheckBox) != null)
				provider = new CheckBoxProvider (c);
			else if ((t = component as TextBox) != null)
				provider = new TextBoxProvider (t);
			else if ((ll = component as LinkLabel) != null)
				provider = new LinkLabelProvider (ll);
			else if ((l = component as Label) != null)
				provider = new LabelProvider (l);
			else if ((sb = component as StatusBar) != null)
				provider = new StatusBarProvider (sb);
			else if ((cb = component as ComboBox) != null)
				provider = new ComboBoxProvider (cb);
			else if ((lb = component as ListBox) != null)
				provider = new ListBoxProvider (lb);
			else if ((scb = component as ScrollBar) != null) {
				if ((lb = component.Container as ListBox) != null) {
					provider = new ListBoxProvider.ListBoxScrollBarProvider (scb);
					((FragmentRootControlProvider) provider).InitializeChildControlStructure ();
				} else
					provider = new ScrollBarProvider (scb);
			} else if ((pb = component as PictureBox) != null)
				provider = new PaneProvider (pb);
			//NOTE: The following providers are Component-based meaning that
			//can be shared.
			//TODO: Add HelpProviderProvider
			else if ((ep = component as ErrorProvider) != null) {
				provider = new ErrorProviderProvider (ep);
				isComponentBased = true;
			} else if ((tt = component as ToolTip) != null) {
				provider = new ToolTipProvider (tt);
				isComponentBased = true;
			} else //TODO: We have to solve the problem when there's a Custom control
				throw new NotImplementedException ("Provider not implemented for control");
			
			if (provider != null) {
				// TODO: Make tracking in dictionary optional
				componentProviders [component] = provider;
				if (initializeEvents)
					provider.InitializeEvents ();
				
				//TODO: Be aware that this may lead to calling
				//      InitializeChildControlStructure several times
				if (initializeChildControlStructure 
				    && component.Container == null 
				    && provider is FragmentRootControlProvider) {
					FragmentRootControlProvider root =
						provider as FragmentRootControlProvider;
					Console.WriteLine ("ProviderFactory: {0}", root.GetType ());
					root.InitializeChildControlStructure ();
				}
				
				//Read comment in ReleaseProvider method for detailed information.
				if (isComponentBased == true)
					sharedComponents.Add (component, 1);
				
				return provider;
			} else
				return null;
		}
		
		//Control-based Providers aren't shared, meaning that you can't
		//associate the Control with more than one Control/Component, so
		//releasing it doesn't impact in other providers, however Component-based
		//Providers are shared, meaning that you can associate the Component with 
		//one or more Controls, for example the ToolTip and HelpProvider.
		//We are releasing the Component-based when the number of sharedComponents
		//reaches 1, that way we aren't creating the provider over and over.
		public static void ReleaseProvider (Component component)
		{
			IRawElementProviderFragment provider;
			if (componentProviders.TryGetValue (component, 
			                                    out provider) == true) {
				if (sharedComponents.ContainsKey (component) == true) {
					if (sharedComponents [component] == 1) {
						componentProviders.Remove (component);
						((FragmentControlProvider) provider).Terminate ();
						sharedComponents.Remove (component);
					} else
						sharedComponents [component] = sharedComponents [component] - 1;
				} else {
					componentProviders.Remove (component);
					((FragmentControlProvider) provider).Terminate ();
				}
			}
		}
		
		private static IRawElementProviderFragment FindProvider (Component component)
		{
			IRawElementProviderFragment provider;
			
			if (component == null)
				return null;
			else if (componentProviders.TryGetValue (component, out provider)) {
				if (sharedComponents.ContainsKey (component) == true) {
					sharedComponents [component] = sharedComponents [component] + 1;
				}
				return provider;
			}

			return null;
		}
		
		#endregion
	}
}
