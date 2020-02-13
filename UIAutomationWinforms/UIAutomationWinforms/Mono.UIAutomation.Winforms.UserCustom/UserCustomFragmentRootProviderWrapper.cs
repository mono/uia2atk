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

using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.UserCustom
{
	internal class UserCustomFragmentRootProviderWrapper : UserCustomFragmentProviderWrapper, IRawElementProviderFragmentRoot
	{
		public UserCustomFragmentRootProviderWrapper (IRawElementProviderFragmentRoot wrappedFragmentRootProvider)
			: base (wrappedFragmentRootProvider)
		{
		}

		#region IRawElementProviderFragmentRoot

		/// TODO: Check for child fragments. Can this logic be generalized?
		public IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			return null;
		}

		/// TODO: Check for child fragments. Can this logic be generalized?
		public IRawElementProviderFragment GetFocus ()
		{
			return null;
		}

		#endregion  // IRawElementProviderFragmentRoot
	}
}