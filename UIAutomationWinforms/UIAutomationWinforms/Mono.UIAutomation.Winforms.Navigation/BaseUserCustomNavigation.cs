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
// Copyright (c) 2020 AxxonSoft (http://axxonsoft.com)
//
// Authors:
//   Nikita Voronchev <nikita.voronchev@ru.axxonsoft.com>
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.UserCustom;

namespace Mono.UIAutomation.Winforms.Navigation
{
	internal abstract class BaseUserCustomNavigation
	{
		protected UserCustomFragmentProviderWrapper UserCustomProviderWrapper { get; private set; } = null;

		private readonly List<UserCustomFragmentProviderWrapper> Children = new List<UserCustomFragmentProviderWrapper> ();

		public abstract IRawElementProviderFragment Parent { get; protected set; }
		public abstract IRawElementProviderFragment NextSibling { get; }
		public abstract IRawElementProviderFragment PreviousSibling { get; }

		public UserCustomFragmentProviderWrapper FirstChild
		{
			get 
			{
				var edges = SyncChildren ();
				return edges.First;
			}
		}

		public UserCustomFragmentProviderWrapper LastChild
		{
			get 
			{
				var edges = SyncChildren ();
				return edges.Last;
			}
		}

		public void SetUserCustomProviderWrapper (UserCustomFragmentProviderWrapper wrapper)
		{
			if (UserCustomProviderWrapper != null)
				throw new Exception ($"UserCustomProviderWrapper={UserCustomProviderWrapper} wrapper={wrapper}");
			UserCustomProviderWrapper = wrapper;
		}

		private bool _terminated = false;

		public void Reinitialize ()
		{
			Terminate ();
			_terminated = false;
			SyncChildren ();
		}

		public void Terminate ()
		{
			if (_terminated)
				return;
			_terminated = true;

			foreach (var child in Children) {
				UserCustomProviderFabric.Forget (child.WrappedFragmentProvider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved, child);
			}
			Children.Clear ();

			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated, UserCustomProviderWrapper);
		}

		private ChildrenCollectionEdges SyncChildren ()
		{
			if (_terminated)
				return new ChildrenCollectionEdges (null, null);;

			var current = InterateUserCustomChildren ().ToArray ();

			foreach (var child in Children.Where (ch => !current.Contains (ch.WrappedFragmentProvider)).ToArray ()) {
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved, child);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated, child.Navigation.Parent);

				Children.Remove (child);
				UserCustomProviderFabric.Forget (child.WrappedFragmentProvider);
			}

			foreach (var x in current.Except (Children.Select (ch => ch.WrappedFragmentProvider)).ToArray ()) {
				UserCustomFragmentProviderWrapper newWrapper = null;
				if (x is IRawElementProviderFragmentRoot fragmentRoot)
					newWrapper = UserCustomProviderFabric.GetCustomFragmentRoot (fragmentRoot);
				else if (x is IRawElementProviderFragment fragment)
					newWrapper = UserCustomProviderFabric.GetCustomFragment (fragment);
				if (newWrapper != null) {
					newWrapper.Navigation.Parent = this.UserCustomProviderWrapper;
					Children.Add (newWrapper);

					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded, newWrapper);
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated, newWrapper.Navigation.Parent);
				}
			}

			var first = Children.FirstOrDefault (ch => ch.WrappedFragmentProvider == current.FirstOrDefault ());
			var last = Children.LastOrDefault (ch => ch.WrappedFragmentProvider == current.LastOrDefault ());
			var edges = new ChildrenCollectionEdges (first, last);
			return edges;
		}

		private IEnumerable<IRawElementProviderFragment> InterateUserCustomChildren ()
		{
			var thisUcp = UserCustomProviderWrapper.WrappedFragmentProvider;

			var ucp = thisUcp.Navigate (NavigateDirection.FirstChild);
			if (ucp == null)
				yield break;

			var lastUpc = thisUcp.Navigate (NavigateDirection.LastChild);
			while (true)
			{
				yield return ucp;
				if (ucp == lastUpc)
					yield break;
				ucp = ucp.Navigate (NavigateDirection.NextSibling);
			}
		}

		private struct ChildrenCollectionEdges
		{
			public readonly UserCustomFragmentProviderWrapper First;
			public readonly UserCustomFragmentProviderWrapper Last;

			public ChildrenCollectionEdges(UserCustomFragmentProviderWrapper first, UserCustomFragmentProviderWrapper last)
			{
				First = first;
				Last = last;
			}
		}
	}
}