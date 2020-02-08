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
using System.ComponentModel;
using System.Linq;

namespace Mono.UIAutomation.Winforms.Navigation
{
	internal partial class Navigation
	{
		private readonly Chain _chainWinforms = new Chain ();
		private readonly Chain _chainCustoms = new Chain ();

		public Navigation (FragmentControlProvider provider)
		{
			Provider = provider;
			Chain.LinkOrderedChains (_chainWinforms, _chainCustoms);
		}

		public readonly FragmentControlProvider Provider;
		public FragmentControlProvider Parent { get; protected set; }
		public  FragmentControlProvider PreviousProvider { get; protected set; }
		public  FragmentControlProvider NextProvider { get; protected set; }
		public FragmentControlProvider FirstChildProvider => _chainWinforms.First ?? _chainCustoms.First;
		public FragmentControlProvider LastChildProvider => _chainCustoms.Last ?? _chainWinforms.Last;

		public void InsertChild (int index, FragmentControlProvider newChild)
		{
			if (newChild == null)
				throw new ArgumentNullException ("newChild");
			if (!newChild.Navigation.IsCleared ())
				RaiseNewChildIsClearedError (newChild);

			if (newChild is FragmentProviderWrapper)
			{
				var relativeIndex = index - _chainWinforms.Count;
				_chainCustoms.SafeInsertAt (relativeIndex, newChild);
			}
			else
			{
				_chainWinforms.SafeInsertAt (index, newChild);
			}
			newChild.Navigation.Parent = this.Provider;
		}

		public void AppendChild (FragmentControlProvider newChild)
		{
			if (newChild == null)
				throw new ArgumentNullException ("newChild");
			if (!newChild.Navigation.IsCleared ())
				RaiseNewChildIsClearedError (newChild);

			if (newChild is FragmentProviderWrapper)
				_chainCustoms.AppendToEnd (newChild);
			else
				_chainWinforms.AppendToEnd (newChild);
			newChild.Navigation.Parent = this.Provider;
		}

		public void RemoveChild (FragmentControlProvider child)
		{
			if (child == null)
				throw new ArgumentNullException ("child");

			if (child is FragmentProviderWrapper)
				_chainCustoms.Remove (child);
			else
				_chainWinforms.Remove (child);
			child.Navigation.Clear ();
		}

		public int ChildrenCount => _chainWinforms.Count + _chainCustoms.Count;

		public int TryGetChildIndex (FragmentControlProvider child)
		{
			if (child is FragmentProviderWrapper)
			{
				var relativeIndex = _chainCustoms.TryIndexOf (child);
				if (relativeIndex == -1)
					return -1;
				return _chainWinforms.Count + relativeIndex;
			}
			else
			{
				return _chainWinforms.TryIndexOf (child);
			}
		}

		public FragmentControlProvider GetChild (int index)
		{
			var relativeIndex = index - _chainWinforms.Count;
			if (relativeIndex >= 0)
				return _chainCustoms.GetByIndex (relativeIndex);
			else
				return _chainWinforms.GetByIndex (index);
		}

		public FragmentControlProvider TryGetChild (Component childComponent)
		{
			return _chainWinforms.TryGetByComponentOfProvider (childComponent) ?? _chainCustoms.TryGetByComponentOfProvider (childComponent);
		}

		public FragmentControlProvider[] GetChildren ()
		{
			return _chainWinforms.ToList ().Concat (_chainCustoms.ToList ()).ToArray ();
		}

		public FragmentControlProvider[] GetWinformsChildren ()
		{
			return _chainWinforms.ToList ().ToArray ();
		}

		public FragmentProviderWrapper[] GetCustomChildren ()
		{
			return _chainCustoms.ToList ().Cast<FragmentProviderWrapper> ().ToArray ();
		}

		public virtual void Clear ()
		{
			this.Provider.NavigateEachChildProvider (child =>
			{
				if (child is FragmentControlProvider p)
					p.Navigation.Clear ();
			});

			Parent = null;
			PreviousProvider = null;
			NextProvider = null;

			_chainWinforms.Clear ();
			_chainCustoms.Clear ();
		}

		public bool IsCleared ()
		{
			return Parent == null && PreviousProvider == null  && NextProvider == null; // && _chainWinforms.Count == 0 && _chainCustoms.Count == 0;
		}

		public bool ChildrenContains (FragmentControlProvider child)
		{
			return TryGetChildIndex (child) >= 0;
		}

		public bool ChildrenContains (Component childComponent)
		{
			return TryGetChild (childComponent) != null;
		}

		public override string ToString ()
		{
			return $"<{this.GetType ()}:{Provider}>";
		}

		private string RaiseNewChildIsClearedError (FragmentControlProvider newChild)
		{
			var errMsg =
				$"RaiseNewChildIsClearedError(): this.Provider={this.Provider} newChild={newChild}"
				+ Environment.NewLine + "  this.Provider.Navigation.ToStringDetailed()" + Environment.NewLine + this.Provider.Navigation.ToStringDetailed(indent: 4)
				+ Environment.NewLine + "  newChild.Navigation.ToStringDetailed()" + Environment.NewLine + newChild.Navigation.ToStringDetailed(indent: 4);
			throw new ArgumentException (errMsg);
		}

		private string ToStringDetailed (int indent = 0)
		{
			var sindent =  new String(' ', indent);;
			return sindent + $"<{this.GetType ()}:{Provider}>" + Environment.NewLine
				+ sindent +  $"  Parent =               {Parent}" + Environment.NewLine
				+ sindent +  $"  PreviousProvider =     {PreviousProvider}" + Environment.NewLine
				+ sindent +  $"  NextProvider =         {NextProvider}" + Environment.NewLine
				+ sindent +  $"  _chainWinforms.Count = {_chainWinforms.Count}" + Environment.NewLine
				+ sindent +  $"  _chainCustoms.Count =  {_chainCustoms.Count}";
		}
	}
}
