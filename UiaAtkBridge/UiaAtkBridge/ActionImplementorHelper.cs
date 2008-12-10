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
//      Mike Gorse <mgorse@novell.com>
// 

using System.Collections.Generic;

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	internal delegate bool ActionDelegate ();

	internal class ActionDescription
	{
		internal String name;
		internal String localizedName;
		internal String description;
		internal ActionDelegate DoAction;
		internal ActionDescription (string name, String localizedName, String description, ActionDelegate doAction)
		{
			this.name = name;
			this.localizedName = localizedName;
			this.description = description;
			this.DoAction = doAction;
		}
	}

	internal class ActionImplementorHelper
	{
		private List<ActionDescription> actions;

		public ActionImplementorHelper ()
		{
			actions = new System.Collections.Generic.List<ActionDescription> ();
		}

		public void Add (String name, String localizedName, String description, ActionDelegate doAction)
		{
			actions.Add (new ActionDescription (name, localizedName, description, doAction));
		}

		public bool Remove (String name)
		{
			foreach (ActionDescription ad in actions) {
				if (ad.name == name) {
					actions.Remove (ad);
					return true;
				}
			}
			return false;
		}

		public int NActions {
			get { return actions.Count; }
		}

		public String GetName (int action)
		{
			if (action < 0 || action >= NActions)
				return null;
			return actions[action].name;
		}

		public String GetLocalizedName (int action)
		{
			if (action < 0 || action >= NActions)
				return null;
			return actions[action].localizedName;
		}

		public String GetDescription (int action)
		{
			if (action < 0 || action >= NActions)
				return null;
			return actions[action].description;
		}

		public bool SetDescription (int action, String description)
		{
			if (action < 0 || action >= NActions)
				return false;
			actions[action].description = description;
			return true;
		}

		public bool DoAction (int action)
		{
			if (action < 0 || action >= NActions)
				return false;
			return actions[action].DoAction ();
		}
	}
}
