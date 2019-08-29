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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Navigation
{
	internal class Navigation
	{
		#region Constructor

		public Navigation (FragmentControlProvider provider)
		{
			Provider = provider;
		}

		#endregion

		#region Public Methods

		public FragmentControlProvider Provider { get; private set; }

		public FragmentControlProvider Parent { get; private set; }

		public virtual void AppendChild (FragmentControlProvider newChild)
		{
			LinkOrderedProviders (_lastChild, newChild);

			SetAsLastChild (newChild);
			if (_firstChild == null)
				SetAsFirstChild (newChild);

			IncrementCounterAndSetParent (newChild);
		}

		public virtual void InsertChild (int index, FragmentControlProvider newChild)
		{
			if (!IsIndexValid (index))
				throw new IndexOutOfRangeException (String.Format (
					"index={0}; _childrenCount={1}", index, _childrenCount));

			var currentAtIndex = GetChild (index);
			var correntPrevious = currentAtIndex.Navigation._previousProvider;

			LinkOrderedProviders (correntPrevious, newChild);
			LinkOrderedProviders (newChild, currentAtIndex);
			if (index == 0)
				SetAsFirstChild (newChild);

			IncrementCounterAndSetParent (newChild);
		}

		public virtual void RemoveChild (FragmentControlProvider child)
		{
			LinkOrderedProviders (child.Navigation._previousProvider, child.Navigation._nextProvider);

			if (child.Navigation._previousProvider == null)
				SetAsFirstChild (child.Navigation._nextProvider);

			if (child.Navigation._nextProvider == null)
				SetAsLastChild (child.Navigation._previousProvider);

			child.Navigation.Clear ();

			--_childrenCount;
		}

		public int ChildrenCount
		{
			get { return _childrenCount; }
		}

		public bool ChildrenContains (FragmentControlProvider child)
		{
			return TryGetChildIndex (child) >= 0;
		}

		public bool ChildrenContains (Component childComponent)
		{
			return TryGetChild (childComponent) != null;
		}

		public int TryGetChildIndex (FragmentControlProvider child)
		{
			if (child == null)
				throw new ArgumentNullException("child");

			var index = 0;
			var p = _firstChild;

			while (p != null)
			{
				if (p.Equals (child))
					return index;
				p = p.Navigation._nextProvider;
				++index;
			}

			return -1;
		}

		public FragmentControlProvider GetChild (int index)
		{
			var p = _firstChild;
			for (var i = 0; i < index; ++i)
			{
				p = p.Navigation._nextProvider;
			}
			return p;
		}

		public FragmentControlProvider TryGetChild (Component childComponent)
		{
			if (childComponent == null)
				return null;

			var p = _firstChild;
			while (p != null)
			{
				if (childComponent.Equals (p.Component))
					return p;
				p = p.Navigation._nextProvider;
			}

			return null;
		}

		public FragmentControlProvider[] GetChildren ()
		{
			var o = new List<FragmentControlProvider>();
			var p = _firstChild;
			while (p != null)
			{
				o.Add (p);
				p = p.Navigation._nextProvider;
			}
			return o.ToArray ();
		}

		public IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			switch (direction)
			{
				case NavigateDirection.Parent:
					// TODO: Review this
					// "Fragment roots do not enable navigation to
					// a parent or siblings; navigation among fragment
					// roots is handled by the default window providers."
					// Source: http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.navigate.aspx
					return ((IRawElementProviderFragment) Parent) ?? Provider.FragmentRoot;
				case NavigateDirection.FirstChild:
					return _firstChild;
				case NavigateDirection.LastChild:
					return _lastChild;
				case NavigateDirection.NextSibling:
					return _nextProvider;
				case NavigateDirection.PreviousSibling:
					return _previousProvider;
				default:
					return null;
			}
		}

		private void Clear ()
		{
			this.Provider.NavigateEachChildProvider (child =>
			{
				if (child is FragmentControlProvider p)
					p.Navigation.Clear ();
			});
			Parent = null;
			_firstChild = null;
			_lastChild = null;
			_previousProvider = null;
			_nextProvider = null;
			_childrenCount = 0;
		}

		public bool IsIndexValid (int index)
		{
			return index >= 0 && index < _childrenCount;
		}

		#endregion

		#region Private Methods

		private void SetAsLastChild (FragmentControlProvider child)
		{
			_lastChild = child;
			if (child != null)
				child.Navigation._nextProvider = null;
		}

		private void SetAsFirstChild (FragmentControlProvider child)
		{
			_firstChild = child;
			if (child != null)
				child.Navigation._previousProvider = null;
		}


		private void LinkOrderedProviders (FragmentControlProvider p1, FragmentControlProvider p2)
		{
			if (p1 != null)
			{
				p1.Navigation._nextProvider = p2;
			}
			if (p2 != null)
			{
				p2.Navigation._previousProvider = p1;
			}
		}

		private void IncrementCounterAndSetParent (FragmentControlProvider newChild)
		{
			newChild.Navigation.Parent = this.Provider;
			++_childrenCount;
		}

		#endregion

		#region Private Fields

		// It seems, some controls (like ComboBox) have numerous Providers with the same Component.
		// Some providers may relates to the DummyComponent. Threfore, the Provider is a primary key
		// in any collection. One can get a Component (real or dummy) by a Provider. But not vice versa
		// every time.
		private FragmentControlProvider _firstChild = null;
		private FragmentControlProvider _lastChild = null;
		private int _childrenCount = 0;

		private FragmentControlProvider _previousProvider;
		private FragmentControlProvider _nextProvider;

		#endregion
	}
}
