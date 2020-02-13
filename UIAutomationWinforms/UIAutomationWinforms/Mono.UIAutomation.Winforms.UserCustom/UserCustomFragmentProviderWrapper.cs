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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms.UserCustom
{
	internal class UserCustomFragmentProviderWrapper : IRawElementProviderFragment
	{
		public readonly IRawElementProviderFragment WrappedFragmentProvider;

		protected readonly int runtimeId;
		public BaseUserCustomNavigation Navigation;

		public UserCustomFragmentProviderWrapper (IRawElementProviderFragment wrappedFragmentProvider)
		{
			WrappedFragmentProvider = wrappedFragmentProvider;
			runtimeId = Helper.GetUniqueRuntimeId ();
		}

		public void OnAdded ()
		{
			Navigation.Reinitialize ();
		}

		public void OnRemoved ()
		{
			Terminate ();
		}

		public void Terminate ()
		{
			Navigation.Terminate ();
		}

		public void SetNavigation (BaseUserCustomNavigation navigation)
		{
			if (Navigation != null)
				throw new Exception ($"Navigation={Navigation} navigation={navigation}");
			Navigation = navigation;
			Navigation.SetUserCustomProviderWrapper (this);
		}

		public override string ToString ()
		{
			return $"<{this.GetType ()}:{WrappedFragmentProvider}>";
		}

		#region IRawElementProviderSimple

		public IRawElementProviderSimple HostRawElementProvider => WrappedFragmentProvider.HostRawElementProvider;
		
		public ProviderOptions ProviderOptions => WrappedFragmentProvider.ProviderOptions;

		public object GetPatternProvider (int patternId)
		{
			return WrappedFragmentProvider.GetPatternProvider (patternId);
		}

		public object GetPropertyValue (int propertyId)
		{
			var propertyValue = WrappedFragmentProvider.GetPropertyValue (propertyId);

			if (propertyId == AutomationElement.RuntimeIdProperty.Id)
			{
				var empty = propertyValue == null || (propertyValue as int[])?.Length == 0;
				if (empty)
					return GetRuntimeId ();
			}

			if (propertyValue != null)
				return propertyValue;

			if (propertyId == AutomationElement.ControlTypeProperty.Id)
				return ControlType.Custom.Id;
			if (propertyId == AutomationElement.BoundingRectangleProperty.Id)
				return BoundingRectangle;

			return null;
		}

		#endregion  // IRawElementProviderSimple

		#region IRawElementProviderFragment

		public System.Windows.Rect BoundingRectangle => WrappedFragmentProvider.BoundingRectangle;

		public IRawElementProviderFragmentRoot FragmentRoot => (IRawElementProviderFragmentRoot) ProviderFactory.FindUserCustomWrapper (WrappedFragmentProvider.FragmentRoot);

		public IRawElementProviderSimple[] GetEmbeddedFragmentRoots () 
		{
			return null;
		}

		public int[] GetRuntimeId ()
		{
			return new int[] { AutomationInteropProvider.AppendRuntimeId, runtimeId };
		}

		public IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			switch (direction)
			{
				case NavigateDirection.Parent:
					return Navigation?.Parent;
				case NavigateDirection.FirstChild:
					return Navigation?.FirstChild;
				case NavigateDirection.LastChild:
					return Navigation?.LastChild;
				case NavigateDirection.NextSibling:
					return Navigation?.NextSibling;
				case NavigateDirection.PreviousSibling:
					return Navigation?.PreviousSibling;
				default:
					return null;
			}
		}

		public void SetFocus ()
		{
		}

		#endregion  // IRawElementProviderFragment
	}
}