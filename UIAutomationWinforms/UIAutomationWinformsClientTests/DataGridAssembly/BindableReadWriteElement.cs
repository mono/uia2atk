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

namespace DataGridAssembly
{
    [Serializable]
    public class BindableReadWriteElement
    {
        public BindableReadWriteElement () : this (0, string.Empty)
        {
        }

        public BindableReadWriteElement (int integer, string name)
        {
            Integer = integer;
            Name = name;
        }

        public override string ToString ()
        {
            return string.Format ("{0}. Integer: {1}. Name {2}",
                GetType ().Name, Integer, Name);
        }

        #region IEquatable<BindableReadonlyElement> Members

        public bool Equals (BindableReadWriteElement other)
        {
            return other.Name == Name && other.Integer == Integer;
        }

        #endregion

        public override bool Equals (object obj)
        {
            BindableReadWriteElement element = obj as BindableReadWriteElement;
            if (element == null)
                return false;
            return Equals (element);
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }
        
        public int Integer { get; set; }
        public string Name { get; set; }
    }
}
