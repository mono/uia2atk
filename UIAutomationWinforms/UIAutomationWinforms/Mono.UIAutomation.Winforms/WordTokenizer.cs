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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms
{

	public class WordTokenizer
	{
		
#region Constructor

		public WordTokenizer (string message)
		{
			this.message = message;
		}

#endregion
		
#region Backwards Methods
		
		public WordTokenCollection Backwards (int index) {
			return Backwards (index, -1);
		}
		
		public WordTokenCollection Backwards (int index, int maxLength) {
			WordTokenCollection tokens = new WordTokenCollection ();
			int newIndex = 0;
			int oldIndex = 0;
			char separator = ' ';
			bool wasSpace = false;
			int spacesCounter = 1;
			
			if (index <= 0)
				return tokens;

			do {
				if (index == -1) { //We have reached last found space
					if (AddSpaces (tokens, newIndex + spacesCounter, 
					               ref wasSpace, ref spacesCounter) == false) {
						if (tokens.Count == maxLength)
							break;

						tokens.Add (new WordToken (message.Substring (newIndex + 1, index - newIndex),
						                           newIndex - 1));
					}
					break;
				}

				newIndex = message.LastIndexOf (separator, index, index + 1);
				if (newIndex == -1) {
					AddSpaces (tokens, oldIndex + spacesCounter, ref wasSpace, 
					           ref spacesCounter);
					if (tokens.Count == maxLength)
						break;

					tokens.Add (new WordToken (message.Substring (0, index + 1), 0));
					break;
				}

				if (oldIndex - newIndex == 1) { //Previous index is also space
					wasSpace = true;
					oldIndex = newIndex;
					index = newIndex - 1;
					spacesCounter++;
					continue;
				} else if (newIndex - index == 0) { //First index is space
					wasSpace = true;
					oldIndex = newIndex;
					index = newIndex - 1;
					continue;
				}

				AddSpaces (tokens, oldIndex + spacesCounter, ref wasSpace, 
				           ref spacesCounter);
				if (tokens.Count == maxLength)
					break;

				tokens.Add (new WordToken (message.Substring (newIndex + 1, index - newIndex),
				                           newIndex + 1));
				if (tokens.Count == maxLength)
					break;

				oldIndex = newIndex;
				index = newIndex - 1;

			} while (true);

			return tokens;
		}
		
#endregion
		
#region Forward Methods
		
		public WordTokenCollection Forward (int index) {
			return Forward (index, -1);
		}

		public WordTokenCollection Forward (int index, int maxLength) {
			WordTokenCollection tokens = new WordTokenCollection ();
			int newIndex = 0;
			int oldIndex = 0;
			int spacesCounter = 1;
			char separator = ' ';
			bool wasSpace = false;
			
			if (index >= message.Length)
				return tokens;

			do {
				newIndex = message.IndexOf (separator, index);
				if (newIndex == -1) {
					//We only add substring if there aren't spaces
					AddSpaces (tokens, index, ref wasSpace, ref spacesCounter);
					if (tokens.Count == maxLength)
						break;

					if (index != message.Length)
						tokens.Add (new WordToken (message.Substring (index), index));
					break;
				}

				if (newIndex - oldIndex == 1) { //Previous index is also space
					wasSpace = true;
					oldIndex = newIndex;
					index = newIndex + 1;
					spacesCounter++;
					continue;
				} else if (newIndex - index == 0) { //First index is space
					wasSpace = true;
					oldIndex = newIndex;
					index = newIndex + 1;
					continue;
				}

				AddSpaces (tokens, index, ref wasSpace, ref spacesCounter);
				if (tokens.Count == maxLength)
					break;
				
				tokens.Add (new WordToken (message.Substring (index, newIndex - index), 
				                           index));
				if (tokens.Count == maxLength)
					break;

				oldIndex = newIndex;
				index = newIndex + 1;

			} while (true);

			return tokens;
		}
		
#endregion
		
#region Private methods

		private bool AddSpaces (WordTokenCollection tokens, int index, 
		                        ref bool wasSpace, ref int spaces) {
			bool previousValue = wasSpace;

			if (wasSpace) {
				tokens.Add (new WordToken (new String (' ', spaces), index - spaces));
				spaces = 1;
				wasSpace = false;
			} else if (tokens.Count > 0)
				tokens.Add (new WordToken (" ", index - spaces));

			return previousValue;
		}
		
#endregion

#region Private fields
		
		private string message;
		
#endregion
	}
}
