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

namespace GailTestApp {

    public class MovingThread : IDisposable
    {
        public MovingThread()
        {
            gThread = new Thread(new ThreadStart(Run));
        }

        bool alreadyStarted = false;

        private void Run()
        {
            int times = 1;
            while (true)
            {
                //Console.WriteLine("going to run for the {0} time", times);
                if (deleg != null)
                    deleg.Invoke();
                lock (this.forState)
                    this.internalState = ThreadState.Stopped;
                this.wakeUp.Set();
                restart.WaitOne();
                times++;
            }
        }
        private ThreadStart deleg;
        private Thread gThread;
        private System.Threading.EventWaitHandle wakeUp = new System.Threading.AutoResetEvent(false);
        private System.Threading.EventWaitHandle restart = new System.Threading.AutoResetEvent(false);
        private object forState = new object();
        private ThreadState internalState = ThreadState.Stopped;

        public ThreadStart Deleg
        {
            set { deleg = value; }
        }

        public ThreadState ThreadState
        {
            get { return gThread.ThreadState; }
        }

        public void JoinUntilSuspend()
        {
            bool wait = false;
            lock (forState)
            {
                if (this.internalState != ThreadState.Stopped)
                    wait = true;
            }
            if (wait)
                wakeUp.WaitOne();
        }

        public void Start()
        {
            if (alreadyStarted)
            {
                lock (forState)
                    this.internalState = ThreadState.Running;
                restart.Set();
            }
            else
            {
                alreadyStarted = true;
                lock (forState)
                    this.internalState = ThreadState.Running;
                this.gThread.Start();
            }
        }

        public void Dispose()
        {
            gThread.Abort();
            wakeUp.Close();
            restart.Close();
        }
    }

	
	public class MainClass
	{
		static MainWindow win = null;
		
		private static void Main (string[] args)
		{
			Start ();
		}
		
		private static void Start () {
			Start (null);
		}
		
		public static void Start (MovingThread guiThread)
		{
			if (guiThread.ThreadState == ThreadState.Running)
				return;

			Console.WriteLine (guiThread.ThreadState.ToString());
			
			if (guiThread != null) {

				guiThread.Deleg = Run;
				guiThread.Start ();
				
				//little hack (it doesn't matter, it's just for the nunit tests) in
				//order to wait for Gtk initialization
				Thread.Sleep (5000);
			}
			else
			{
				Run ();
			}
		}
		
		private static void Run () {
			Application.Init ();
			win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
		
		public static Gtk.Label GiveMeARealLabel (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealLabel ();
		}
		
		public static Gtk.Button GiveMeARealButton (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealButton ();
		}
		
		public static Gtk.Button GiveMeARealCheckBox (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealCheckBox ();
		}
		
		public static Gtk.ComboBox GiveMeARealComboBox (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealComboBox ();
		}
		
		public static Gtk.RadioButton GiveMeARealRadioButton (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealRadioButton ();
		}
		
		public static Gtk.Entry GiveMeARealEntry (MovingThread guiThread)
		{
			if (win == null)
				Start (guiThread);
			
			return win.GiveMeARealEntry ();
		}
		
		public static void Kill () 
		{
			win.Destroy ();
			win.Dispose ();
			Application.Quit ();
		}
	}
}
