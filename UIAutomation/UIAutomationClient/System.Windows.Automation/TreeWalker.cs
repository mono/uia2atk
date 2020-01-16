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
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Mono.UIAutomation.Source;
using Mono.UIAutomation.Services;

namespace System.Windows.Automation
{
	public sealed partial class TreeWalker
	{
		#region Private Fields
		private Object directChildrenLock = new Object ();
		private List<AutomationElement> directChildren;

		private static bool inSourceRootElementChanged = false;
		private static ConcurrentQueue<IAutomationSource> pendingRootChangeSources = new ConcurrentQueue<IAutomationSource> ();
		#endregion

		#region Static Constructor
		static TreeWalker ()
		{
			RawViewWalker = new TreeWalker (Automation.RawViewCondition);
			InitializeRootElements ();

			ControlViewWalker = new TreeWalker (Automation.ControlViewCondition);
			ContentViewWalker = new TreeWalker (Automation.ContentViewCondition);
		}
		#endregion

		#region Public Constructor
		public TreeWalker (Condition condition)
		{
			if (condition == null)
				throw new ArgumentNullException ("condition");
			Condition = condition;
		}
		#endregion

		#region Private Static Methods

		private static void InitializeRootElements ()
		{
			lock (RawViewWalker.directChildrenLock) {
				var pidElementMapping = new Dictionary<int, IElement> ();
				RawViewWalker.directChildren = new List<AutomationElement> ();
				foreach (IAutomationSource source in SourceManager.GetAutomationSources ()) {
					AddUniqueRootElements (RawViewWalker.directChildren,
					                       source,
					                       pidElementMapping);
					source.RootElementsChanged += (s, e) => OnSourceRootElementChanged ((IAutomationSource) s);
				}
			}
		}

		private static void OnSourceRootElementChanged (IAutomationSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (inSourceRootElementChanged) {
				pendingRootChangeSources.Enqueue (source);
				return;
			}

			lock (RawViewWalker.directChildrenLock) {
				inSourceRootElementChanged = true;
				var rootElements = new List<AutomationElement> ();
				var pidElementMapping = new Dictionary<int, IElement> ();
				foreach (AutomationElement element in RawViewWalker.directChildren) {
					if (element.SourceElement.AutomationSource != source) {
						try {
							pidElementMapping [element.Current.ProcessId] = element.SourceElement;
						} catch { continue; } // TODO: ElementNotAvailableException
						rootElements.Add (element);
					}
				}
				// We don't handle the cleanup of AutomationElements here, they're
				// handled by each AutomationSource.
				// "Clean up" includes removing event handlers etc.
				AddUniqueRootElements (rootElements, source, pidElementMapping);
				RawViewWalker.directChildren = rootElements;
				inSourceRootElementChanged = false;
			}

			Log.Debug ("Root elements are refreshed, Count: {0}", RawViewWalker.directChildren.Count);
			while (pendingRootChangeSources.TryDequeue (out var s)) {
				OnSourceRootElementChanged (s);
			}
		}

		// TODO: This approach will completely break when the
		//       MoonUiaDbusBridge work is committed. This code
		//       will be replaced with proper source tree merging,
		//       which requires updates to gtk-sharp.
		private static void AddUniqueRootElements (List<AutomationElement> rootElements,
		                                           IAutomationSource source,
		                                           Dictionary<int, IElement> pidElementMapping)
		{
			const string at_spi = "at-spi";
			foreach (IElement sourceElement in source.GetRootElements ()) {
				int pid;
				string sourceFid;
				try {
					pid = sourceElement.ProcessId;
					sourceFid = sourceElement.FrameworkId;
				} catch { continue; } // TODO: ElementNotAvailableException

				// NOTE: This is not a complete mapping, since
				//       one process could generate multiple root
				//       elements. But it's sufficient to catch
				//       that at least one element exists for
				//       a given PID.
				if (pidElementMapping.TryGetValue (pid, out IElement found)) {
					string foundFid;
					try {
						foundFid = found.FrameworkId;
					} catch { continue; } // TODO: ElementNotAvailableException

					// Ignore at-spi elements when elements
					// for this process from a preferred
					// framework exist
					if (sourceFid == at_spi &&
					    foundFid != at_spi)
						continue;
					// Remove at-spi elements when elements
					// for this process from a preferred
					// framework exist
					else if (sourceFid != at_spi &&
					         foundFid == at_spi) {
						// TODO: When we fix ElementNotAvailableException,
						//       we'll need to mark these
						//       elements as unavailable.
						rootElements.RemoveAll (e => e.Current.ProcessId == pid &&
						                        e.Current.FrameworkId == at_spi);
					}
				}

				rootElements.Add (SourceManager.GetOrCreateAutomationElement (sourceElement));
				pidElementMapping [pid] = sourceElement;
			}
		}

		#endregion

		#region Public Methods
		public AutomationElement GetParent (AutomationElement element)
		{
			return new TreeIterator (Condition).GetParent (element);
		}

		public AutomationElement GetFirstChild (AutomationElement element)
		{
			return new TreeIterator (Condition).GetFirstChild (element);
		}

		public AutomationElement GetLastChild (AutomationElement element)
		{
			return new TreeIterator (Condition).GetLastChild (element);
		}

		public AutomationElement GetNextSibling (AutomationElement element)
		{
			return new TreeIterator (Condition).GetNextSibling (element);
		}

		public AutomationElement GetPreviousSibling (AutomationElement element)
		{
			return new TreeIterator (Condition).GetPreviousSibling (element);
		}

		public AutomationElement Normalize (AutomationElement element)
		{
			if (element == null)
				throw new ArgumentNullException ("element");
			if (Condition.AppliesTo (element))
				return element;
			if (element == AutomationElement.RootElement)
				// LAMESPEC: This is according to MSDN:
				// http://msdn.microsoft.com/en-us/library/system.windows.automation.treewalker.normalize.aspx
//				return element;
				// This is matching Microsoft's actual implementation:
				return null;
			return Normalize (RawViewWalker.GetParent (element));
		}

		public AutomationElement GetParent (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetParent (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetFirstChild (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetFirstChild (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetLastChild (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetLastChild (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetNextSibling (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetNextSibling (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement GetPreviousSibling (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = GetPreviousSibling (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}

		public AutomationElement Normalize (AutomationElement element, CacheRequest request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			var result = Normalize (element);
			if (result != null)
				return result.GetUpdatedCache (request);
			return result;
		}
		#endregion

		#region Public Properties
		public Condition Condition { get; private set; }
		#endregion

		#region Public Static Fields
		public static readonly TreeWalker RawViewWalker;

		public static readonly TreeWalker ControlViewWalker;

		public static readonly TreeWalker ContentViewWalker;
		#endregion
	}
}
