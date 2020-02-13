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
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms.UserCustom
{
	internal static class UserCustomProviderFabric
	{
		private static Dictionary<IRawElementProviderFragment,UserCustomFragmentProviderWrapper> _customToWrapperMap
			= new Dictionary<IRawElementProviderFragment,UserCustomFragmentProviderWrapper> ();

		public static UserCustomFragmentProviderWrapper GetWinformFragment (
			FragmentControlProvider winformFragment,
			IRawElementProviderFragment userCustomFragment)
		{
			return FindOrMakeWinformCustomWrapper (
				winformFragment,
				userCustomFragment,
				() => new UserCustomFragmentProviderWrapper (userCustomFragment));
		}

		public static UserCustomFragmentRootProviderWrapper GetWinformFragmentRoot (
			FragmentControlProvider winformFragment,
			IRawElementProviderFragmentRoot userCustomFragmentRoot)
		{
			return (UserCustomFragmentRootProviderWrapper) FindOrMakeWinformCustomWrapper (
				winformFragment,
				userCustomFragmentRoot,
				() => new UserCustomFragmentRootProviderWrapper (userCustomFragmentRoot));
		}

		public static UserCustomFragmentProviderWrapper GetCustomFragment (
			IRawElementProviderFragment userCustomFragment)
		{
			return FindOrMakePureCustomWrapper (
				userCustomFragment,
				() => new UserCustomFragmentProviderWrapper (userCustomFragment));
		}

		public static UserCustomFragmentRootProviderWrapper GetCustomFragmentRoot (
			IRawElementProviderFragmentRoot userCustomFragmentRoot)
		{
			return (UserCustomFragmentRootProviderWrapper) FindOrMakePureCustomWrapper (
				userCustomFragmentRoot,
				() => new UserCustomFragmentRootProviderWrapper (userCustomFragmentRoot));
		}

		public static void Forget (IRawElementProviderFragment userCustomFragment)
		{
			var wrapper = _customToWrapperMap [userCustomFragment];
			_customToWrapperMap.Remove (userCustomFragment);
			wrapper.Terminate ();
		}

		public static UserCustomFragmentProviderWrapper Find (IRawElementProviderFragment userCustomFragment)
		{
			if (_customToWrapperMap.TryGetValue (userCustomFragment, out var wrapper))
				return wrapper;
			return null;
		}

		private static UserCustomFragmentProviderWrapper FindOrMakeWinformCustomWrapper (
			FragmentControlProvider winformFragment,
			IRawElementProviderFragment userCustomFragment,
			Func<UserCustomFragmentProviderWrapper> wrapperFabric)
		{
			return FindOrMakeWrapper (
				userCustomFragment,
				wrapperFabric,
				() => new UserCustomWinformNavigation (winformFragment));
		}

		private static UserCustomFragmentProviderWrapper FindOrMakePureCustomWrapper (
			IRawElementProviderFragment userCustomFragment,
			Func<UserCustomFragmentProviderWrapper> wrapperFabric)
		{
			return FindOrMakeWrapper (
				userCustomFragment,
				wrapperFabric,
				() => new UserCustomNavigation ());
		}

		private static UserCustomFragmentProviderWrapper FindOrMakeWrapper (
			IRawElementProviderFragment userCustomFragment,
			Func<UserCustomFragmentProviderWrapper> wrapperFabric,
			Func<BaseUserCustomNavigation> navigationFabric)
		{
			UserCustomFragmentProviderWrapper wrapper;
			if (_customToWrapperMap.TryGetValue (userCustomFragment, out wrapper))
				return wrapper;
			wrapper = wrapperFabric ();
			wrapper.SetNavigation (navigationFabric ());
			_customToWrapperMap[userCustomFragment] = wrapper;

			return wrapper;	
		}
	}
}