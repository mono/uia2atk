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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 
using System;
using Gtk;

using System.Threading;

namespace GailTestApp
{
	public class MainClass
	{
		static MainWindow win = null;
		
		private static void Main (string[] args)
		{
			Start (false);
			Application.Run ();
		}
		
		private static void Start (bool runMainLoopInNewThread)
		{
			Application.Init ();
			win = new MainWindow ();
			win.Show ();
			
			if (runMainLoopInNewThread) {
				new Thread (new ThreadStart (Application.Run)).Start ();
				
				//little hack (it doesn't matter, it's just for the nunit tests) in
				//order to wait for Gtk initialization
				Thread.Sleep (6000);
			}
		}
		
		public static Gtk.Label GiveMeARealLabel ()
		{
			if (win == null)
				Start (true);
			
			return win.GiveMeARealLabel ();
		}
		
		public static Gtk.Button GiveMeARealButton ()
		{
			if (win == null)
				Start (true);
			
			return win.GiveMeARealButton ();
		}
	}
}