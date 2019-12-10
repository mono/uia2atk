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
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal class FragmentProviderWrapper : SimpleProviderWrapper
	{
		public FragmentProviderWrapper (Component component, IRawElementProviderFragment fragmentProvider)
			: base (component, fragmentProvider)
		{
		}

		public IRawElementProviderFragment WrappedFragmentProvider
		{
			get { return (IRawElementProviderFragment) wrappedProvider; }
		}

		protected override System.Windows.Rect BoundingRectangleProperty
		{
			get { return WrappedFragmentProvider.BoundingRectangle; }
		}

		#region IRawElementProviderFragment Interface

		public override IRawElementProviderFragmentRoot FragmentRoot
		{
			get { return null; }  // TODO
		}

		public override IRawElementProviderSimple[] GetEmbeddedFragmentRoots ()
		{
			return new IRawElementProviderSimple[0];  // TODO
		}

		public override int[] GetRuntimeId ()
		{
			var baseId = base.GetRuntimeId ();
			var providerId = WrappedFragmentProvider.GetRuntimeId () ?? new int [0];

			var concatId = new int [baseId.Length + providerId.Length];
			baseId.CopyTo (concatId, 0);
			providerId.CopyTo (concatId, baseId.Length);
			return concatId;
		}

		public override IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			if (direction == NavigateDirection.FirstChild || direction == NavigateDirection.LastChild)
				UpdateCustomProvidersChildrenStructure ();
			return base.Navigate (direction);
		}

		protected override void InsertChildProvider (bool raiseEvent, FragmentControlProvider childProvider, int index)
		{
			if (childProvider is FragmentProviderWrapper)
				InsertChildProvider (raiseEvent, childProvider, index, false);
			else
				base.InsertChildProvider (raiseEvent, childProvider, index);
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			UpdateCustomProvidersChildrenStructure ();
		}

		public override void SetFocus ()
		{
			WrappedFragmentProvider.SetFocus ();
		}

		#endregion

		protected void UpdateCustomProvidersChildrenStructure ()
		{
			var currentCustom = WrappedFragmentProvider.GetNavigatedChildProviders ();
			var storedWrappers = Navigation.GetCustomChildren ();
			var storedCustom = storedWrappers.Select (x => x.WrappedFragmentProvider).ToArray ();

			foreach (var provider in storedWrappers)
			{
				if (!currentCustom.Contains (provider.WrappedFragmentProvider))
					HandleChildComponentRemoved (provider.Component);
			}

			foreach (IRawElementProviderFragment child in currentCustom)
			{
				if (!storedCustom.Contains (child))
				{
					var customComponent = new UserCustomComponent (child, this);
					HandleChildComponentAdded (customComponent);
				}
			}
		}
	}
}
