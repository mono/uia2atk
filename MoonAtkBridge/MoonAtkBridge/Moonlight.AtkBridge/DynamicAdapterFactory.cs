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
				if (rootVisualAdapter == null) {
					// This happens when the XAP is invalid.
					if (Application.Current == null)
						return null;

					rootVisualAdapter = new RootVisualAdapter (
						GetNewRootPeer (Application.Current));
					if (rootVisualAdapter.Peer == null) {
						rootVisualAdapter.Dispose ();
						rootVisualAdapter = null;
						return null;
					}
					activeAdapters.Add (rootVisualAdapter.Peer,
					                    rootVisualAdapter);
				}

				return rootVisualAdapter;
			}
		}
#endregion

#region Public Methods
		public Adapter GetAdapter (AutomationPeer peer)
		{
			return GetAdapter (peer, true);
		}

		public Adapter GetAdapter (AutomationPeer peer, bool create)
		{
			if (peer == null)
				return null;

			// We ignore all requests until the application is
			// completely loaded.
			if (RootVisualAdapter == null)
				return null;

			// Do we already have a running Adapter instance for
			// this peer?
			if (activeAdapters.ContainsKey (peer))
				return activeAdapters [peer];

			if (!create)
				return null;

			// Create a list of all potential implementors that
			// will later be merged to have a list of implementors
			// with unique Atk interfaces.
			List<Type> implementors = new List<Type> ();

			List<PatternInterface> implementorBlacklist = new List<PatternInterface> ();

			// Do we have any explict implementors for this type?
			List<Type> explicitImpls;
			if (explicitImplementors.TryGetValue (peer.GetType (),
			                                      out explicitImpls)) {
				if (explicitImpls != null) {
					foreach (Type t in explicitImpls)
						if (blacklist.ContainsKey (t))
							implementorBlacklist.Add (blacklist [t]);

					implementors.AddRange (explicitImpls);
				}
			}

			// Do we have any implementors for this control type?
			List<Type> controlTypeImpls;
			if (controlTypeImplementors.TryGetValue (peer.GetAutomationControlType (),
			                                         out controlTypeImpls)) {
				if (controlTypeImpls != null) {
					foreach (Type t in controlTypeImpls)
						if (blacklist.ContainsKey (t))
							implementorBlacklist.Add (blacklist [t]);

					implementors.AddRange (controlTypeImpls);
				}
			}

			// Find implementors for the patterns that the peer
			// implements
			List<Type> potentialImpls = new List<Type> ();
			potentialImpls.AddRange (
				GetImplementorsForPeer (peer, implementorBlacklist));

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
				activeAdapters.Add (peer, adapter);
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

			if (adapterType == null) {
				Log.Warn ("Control should have a {0}, but no adapter of that type could be found.", typeName);

				// Fall back to a basic adapter
				adapter = new Adapter (peer);
				activeAdapters.Add (peer, adapter);
				return adapter;
			}

			Log.Debug ("Creating new instance of {0} for {1}",
			           adapterType, peer.GetType ());

			adapter = (Adapter) Activator.CreateInstance (
				adapterType, new object [] { peer }
			);

			activeAdapters.Add (peer, adapter);
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

		public Type [] GetImplementorsForPeer (AutomationPeer peer,
		                                       List<PatternInterface> blacklist)
		{
			List<Type> implementors = new List<Type> ();

			foreach (PatternInterface pattern
			         in GetEnumValues<PatternInterface> ()) {
				if (blacklist.Contains (pattern))
					continue;

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

#region Internal Methods
		// NOTE: We _must_ keep a reference to all unmanaged wrapper
		// objects (e.g.: anything inheriting from GLib.Object) so that
		// we can dispose of the unmanaged reference during our
		// Shutdown phase.  If we forget about them, they will be
		// finalized during the app domain's shutdown sequence and then
		// *queued* using a GLib.Timeout to be disposed in the main
		// loop.  When the timeout is triggered, the
		// PerformQueuedUnrefs method will most likely be called when
		// corlib or other fundamental namespaces have been finalized,
		// and this will trigger a segfault.
		internal void MarkExternalReference (IDisposable o)
		{
			externalReferences.Add (new WeakReference (o, false));
		}

		internal void UnloadAdapters ()
		{
			Log.Info ("Initiating shutdown sequence:");

			Log.Info (" * Stopping all queued idle and timeout handlers...");

			// Halts all timeout-queued unreffing to prevent
			// segfaults.
			GLib.Object.StopUnreffing ();

			// Stop all queued timeout handlers
			//
			// Note that we're leaking GLib.ValueArray objects as
			// they have their own disposal cycle that we can't
			// easily circumvent to ensure that it doesn't segfault
			// our app.
			GLib.Timeout.SuspendTimeouts = true;
			lock (GLib.Source.source_handlers) {
				foreach (uint tag in GLib.Source.source_handlers.Keys)
					GLib.Source.Remove (tag);
			}

			patternImplementors.Clear ();
			explicitImplementors.Clear ();
			adapterTypes.Clear ();

			Log.Info (" * Marking all adapters as defunct...");

			// Mark the adapters as defunct first, and then do a
			// second pass to dispose them later, so that ATs get a
			// chance to release their references to the objects
			// before we remove our managed objects.  Of course,
			// this is quite race-prone, but what are you going to
			// do?
			if (rootVisualAdapter != null) {
				rootVisualAdapter.Foreach (a => {
					if (a.disposed)
						return;

					a.NotifyStateChange (Atk.StateType.Defunct,
							     true);
				}, false);
			}

			Log.Info (" * Disposing of all active adapters...");

			Action<Adapter> disposeChild = (a) => {
				if (a.disposed)
					return;

				Log.Info ("   - Disposing \"{0}\" ({1})",
					  a.Name, a.GetType ());

				activeAdapters.Remove (a.Peer);

				a.Dispose ();
			};

			// Iterate the hierarchy in a depth-first manner,
			// disposing from bottom up.
			if (rootVisualAdapter != null)
				rootVisualAdapter.Foreach (disposeChild, false);

			Log.Info (" * Disposing remaining adapters...");

			// If there are any adapters left dangling, dispose
			// them too.
			foreach (Adapter a in activeAdapters.Values)
				disposeChild (a);

			rootVisualAdapter = null;

			// Dispose of all of our external references now so
			// they aren't queued to be unreffed later.
			Log.Info (" * Freeing all external references...");
			foreach (WeakReference r in externalReferences) {
				if (!r.IsAlive)
					continue;

				IDisposable target = r.Target as IDisposable;
				if (target != null) {
					Log.Info ("   - Disposing {0}", r.Target.GetType ());
					target.Dispose ();
				}
			}

			externalReferences.Clear ();

			// Verify all GLib.Object subclasses have been
			// disposed.  If they're not, print out debugging
			// information.
			GLib.Object.Shutdown ();
			Atk.Util.GetRootHandler = null;

			Log.Info ("Shutdown complete");
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

					if (ipa.IsProvidesSet)
						blacklist [t] = ipa.Provides;

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

		private Dictionary<Type, PatternInterface> blacklist
			= new Dictionary<Type, PatternInterface> ();

		private Dictionary<AutomationControlType, List<Type>> controlTypeImplementors
			= new Dictionary<AutomationControlType, List<Type>> ();

		// Maps generated type name to the generated adapter type
		private Dictionary<string, Type> adapterTypes
			= new Dictionary<string, Type> ();

		private Dictionary<AutomationPeer, Adapter> activeAdapters
			= new Dictionary<AutomationPeer, Adapter> ();

		private RootVisualAdapter rootVisualAdapter;

		private List<WeakReference> externalReferences
			= new List<WeakReference> ();
#endregion
	}
}
