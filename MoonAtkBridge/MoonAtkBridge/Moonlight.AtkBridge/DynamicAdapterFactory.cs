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
//      Andres Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public sealed class DynamicAdapterFactory
	{
#region Public Properties
		public static DynamicAdapterFactory Instance {
			get { return instance; }
		}

		public Adapter RootVisualAdapter {
			get {
				if (rootVisualAdapter == null)
					rootVisualAdapter = new RootVisualAdapter (GetNewRootPeer (Application.Current));

				return rootVisualAdapter;
			}
		}
#endregion

#region Public Methods
		public Adapter GetAdapter (AutomationPeer peer)
		{
			return GetAdapter (peer, true);
		}

		public Adapter GetAdapter (AutomationPeer peer, bool create_peer)
		{
			if (peer == null)
				return null;

			// Do we already have a running Adapter instance for
			// this peer?
			if (activeAdapters.ContainsKey (peer))
				return activeAdapters [peer];

			if (!create_peer)
				return null;

			// Create a list of all potential implementors that
			// will later be merged to have a list of implementors
			// with unique Atk interfaces.
			List<Type> implementors = new List<Type> ();

			// Do we have any explict implementors for this type?
			List<Type> explicitImpls;
			if (explicitImplementors.TryGetValue (peer.GetType (),
			                                      out explicitImpls)) {
				if (explicitImpls != null)
					implementors.AddRange (explicitImpls);
			}

			// Do we have any implementors for this control type?
			List<Type> controlTypeImpls;
			if (controlTypeImplementors.TryGetValue (peer.GetAutomationControlType (),
			                                         out controlTypeImpls)) {
				if (controlTypeImpls != null)
					implementors.AddRange (controlTypeImpls);
			}

			// Find implementors for the patterns that the peer
			// implements
			List<Type> potentialImpls = new List<Type> ();
			potentialImpls.AddRange (
				GetImplementorsForPeer (peer));

			// Create a reverse mapping between Atk interface and
			// implementor to ensure uniqueness.
			//
			// If there are collisions, favor the one with the most
			// Atk interfaces.
			Dictionary<Type, Type> atkInterfaces
				= new Dictionary<Type, Type> ();
			foreach (Type impl in potentialImpls) {
				Type [] ifaces = impl.GetInterfaces ();

				// If the implementor doesn't implement any Atk
				// interfaces, go ahead and add it to our list
				// of implementors.
				if (ifaces.Length == 0) {
					implementors.Add (impl);
					continue;
				}

				foreach (Type i in ifaces) {
					if (i.Namespace == "Atk"
					    && atkInterfaces.ContainsKey (i)
					    && (ifaces.Length <= atkInterfaces [i].GetInterfaces ().Length))
						atkInterfaces.Remove (i);
					else
						atkInterfaces [i] = impl;
				}
			}

			// Add the distinct implementors
			implementors.AddRange (atkInterfaces.Select (x => x.Value).Distinct ());
			implementors.Sort ((a, b) => a.Name.CompareTo (b.Name));

			// Concat the type names together
			string typeName = implementors.Aggregate (String.Empty, (s, t) => s + t.Name)
				+ "Adapter";

			// If we're not implementing anything, just
			// short-circuit the process
			Adapter adapter = null;
			if (implementors.Count () == 0) {
				adapter = new Adapter (peer);
				activeAdapters [peer] = adapter;
				return adapter;
			}

			// See if we've created the type already
			Type adapterType;
			if (!adapterTypes.TryGetValue (typeName,
			                               out adapterType)) {
				Assembly asm = Assembly.GetCallingAssembly ();
				adapterType = asm.GetType (
					String.Format ("Moonlight.AtkBridge.Adapters.{0}",
					               typeName),
					false
				);

				if (adapterType != null)
					adapterTypes [typeName] = adapterType;
			}

			if (adapterType == null)
				return null;

			Log.Debug ("Creating new instance of {0} for {1}",
			           adapterType, peer.GetType ());

			adapter = (Adapter) Activator.CreateInstance (
				adapterType, new object [] { peer }
			);

			activeAdapters [peer] = adapter;
			return adapter;
		}

		public void UnregisterAdapter (AutomationPeer peer)
		{
			Adapter adapter = null;
			if (!activeAdapters.TryGetValue (peer, out adapter))
				return;

			activeAdapters.Remove (peer);

			if (adapter != null)
				adapter.Dispose ();
		}

		public Type [] GetImplementorsForPeer (AutomationPeer peer)
		{
			List<Type> implementors = new List<Type> ();

			foreach (PatternInterface pattern
			         in GetEnumValues<PatternInterface> ()) {
				var impl = peer.GetPattern (pattern);
				if (impl == null)
					continue;

				List<Type> impls;
				if (patternImplementors.TryGetValue (pattern,
				                                     out impls)) {
					implementors.AddRange (impls);
				}
			}

			return implementors.ToArray ();
		}
#endregion

#region Private Methods
		private DynamicAdapterFactory ()
		{
			Application.UIANewApplication += OnNewApplication;
			RegisterPatternImplementors ();
		}

		private void RegisterPatternImplementors ()
		{
			Assembly asm = typeof (DynamicAdapterFactory).Assembly;
			foreach (Type t in asm.GetTypes ()) {
				object [] attrs = t.GetCustomAttributes (
					typeof (ImplementsPatternAttribute),
					false);

				if (t.IsSubclassOf (typeof (Adapter)))
					continue;

				foreach (Attribute attr in attrs) {
					ImplementsPatternAttribute ipa
						= attr as ImplementsPatternAttribute;
					if (ipa == null)
						continue;

					if (ipa.ElementType != null) {
						if (!explicitImplementors.ContainsKey (ipa.ElementType))
							explicitImplementors [ipa.ElementType] = new List<Type> ();

						explicitImplementors [ipa.ElementType].Add (t);
						continue;
					}

					if (ipa.ControlType.HasValue) {
						AutomationControlType controlType = ipa.ControlType.Value;
						if (!controlTypeImplementors.ContainsKey (controlType))
							controlTypeImplementors [controlType] = new List<Type> ();

						controlTypeImplementors [controlType].Add (t);
						continue;
					}

					if (!patternImplementors.ContainsKey (ipa.Pattern)) {
						// Micro-optimization: Most
						// likely, we'll only have 1
						// implementor per pattern
						patternImplementors [ipa.Pattern]
							= new List<Type> (1);
					}

					patternImplementors [ipa.Pattern].Add (t);
				}
			}
		}

		// This is needed as Enum.GetValues isn't available in the
		// SL 2.0 API
		private T [] GetEnumValues<T> ()
		{
			Type enumType = typeof (T);
			if (!enumType.IsEnum)
				throw new ArgumentException (
					String.Format ("Type {0} is not an enum",
					               enumType));

			return enumType.GetFields ()
			               .Where (f => f.IsLiteral)
			               .Select (f => (T) f.GetValue (enumType))
			               .ToArray ();
		}

		// Assumed that type is already in PascalCase as per STYLE
		// guide.
		private string CamelCaseType (Type t)
		{
			string name = t.Name;
			return name.Length > 0 ? Char.ToLower (name [0]) + name.Substring (1)
			                       : String.Empty;
		}

		private void OnNewApplication (object sender, EventArgs args)
		{
			((Application)sender).UIARootVisualSet += OnRootVisualChanged;
		}

		private void OnRootVisualChanged (object sender, EventArgs args)
		{
			if (rootVisualAdapter == null)
				return;
			rootVisualAdapter.UpdatePeer (GetNewRootPeer ((Application)sender));
		}

		private AutomationPeer GetNewRootPeer (Application app)
		{
			AutomationPeer root_peer = null;
			var root_visual = app.RootVisual as FrameworkElement;
			if (root_visual != null){
				root_peer = new WindowAutomationPeer (root_visual);
				activeAdapters [root_peer] = rootVisualAdapter;
			}

			return root_peer;
		}
#endregion

#region Private Fields
		private static DynamicAdapterFactory instance
			= new DynamicAdapterFactory ();

		private Dictionary<PatternInterface, List<Type>> patternImplementors
			= new Dictionary<PatternInterface, List<Type>> ();

		private Dictionary<Type, List<Type>> explicitImplementors
			= new Dictionary<Type, List<Type>> ();

		private Dictionary<AutomationControlType, List<Type>> controlTypeImplementors
			= new Dictionary<AutomationControlType, List<Type>> ();

		// Maps generated type name to the generated adapter type
		private Dictionary<string, Type> adapterTypes
			= new Dictionary<string, Type> ();

		private Dictionary<AutomationPeer, Adapter> activeAdapters
			= new Dictionary<AutomationPeer, Adapter> ();

		private RootVisualAdapter rootVisualAdapter;
#endregion
	}
}
