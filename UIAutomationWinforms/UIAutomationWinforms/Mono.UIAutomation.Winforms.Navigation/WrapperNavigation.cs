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

namespace Mono.UIAutomation.Winforms.Navigation
{
	internal class WrapperNavigation : Navigation
	{
		public WrapperNavigation (FragmentProviderWrapper provider)
			: base (provider)
		{
		}

		public override void AppendChild (FragmentControlProvider newChild)
		{
			if (newChild is FragmentProviderWrapper)
				base.AppendChild (newChild);
			else
				InsertChild (_lastWinformsProviderIndex, newChild);
		}

		public override void InsertChild (int index, FragmentControlProvider newChild)
		{
			int normalizedIndex;
			if (newChild is FragmentProviderWrapper)
			{
				normalizedIndex = NormalizeIndexToNonWinforms (index);
			}
			else
			{
				normalizedIndex = NormalizeIndexToWinforms (index);
				++_lastWinformsProviderIndex;
			}

			base.InsertChild (normalizedIndex, newChild);
		}

		public override void RemoveChild (FragmentControlProvider child)
		{
			if (!(child is FragmentProviderWrapper))
				--_lastWinformsProviderIndex;
			base.RemoveChild (child);
		}

		private int NormalizeIndexToWinforms (int desiredIndex)
		{
			return (desiredIndex > _lastWinformsProviderIndex) ? _lastWinformsProviderIndex : desiredIndex;
		}

		private int NormalizeIndexToNonWinforms (int desiredIndex)
		{
			return (desiredIndex <= _lastWinformsProviderIndex) ? _lastWinformsProviderIndex + 1 : desiredIndex;
		}

		private int _lastWinformsProviderIndex = 0;
	}
}
