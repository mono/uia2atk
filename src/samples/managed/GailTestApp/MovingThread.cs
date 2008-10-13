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
//	  Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Threading;

namespace GailTestApp
{

	internal class WaitHandleDelegate
	{
		ThreadStart d;
		GLib.TimeoutHandler h;
		EventWaitHandle wakeUp;
		
		internal WaitHandleDelegate (ThreadStart d, System.Threading.EventWaitHandle wakeUp)
		{
			if (d == null)
				throw new ArgumentNullException ("d");
			
			this.d = d;
			this.h = null;
			this.wakeUp = wakeUp;
		}

		internal WaitHandleDelegate (GLib.TimeoutHandler h, System.Threading.EventWaitHandle wakeUp)
		{
			if (h == null)
				throw new ArgumentNullException ("h");
			
			this.h = h;
			this.d = null;
			this.wakeUp = wakeUp;
		}
		
		internal bool DoWork ()
		{
			if (d != null)
				d ();
			else
				h ();
			return wakeUp.Set ();
		}
	}

	
	public class MovingThread : IDisposable
	{
		public MovingThread ()
		{
			GLib.ExceptionManager.UnhandledException += new GLib.UnhandledExceptionHandler (HandleException);
			gThread = new Thread (new ThreadStart (Run));
		}

		static Exception exceptionHappened = null;
		static EventWaitHandle lastHandle = null;

		public Exception ExceptionHappened {
			get { return exceptionHappened; }
		}
		
		static void HandleException (GLib.UnhandledExceptionArgs args)
		{
			args.ExitApplication = false;
			exceptionHappened = (Exception)args.ExceptionObject;
			lastHandle.Set ();
		}
		
		bool alreadyStarted = false;

		private void Run ()
		{
			int times = 1;
			while (true)
			{
				if (deleg != null)
					deleg.Invoke ();
				lock (this.forState)
					this.internalState = ThreadState.Stopped;
				this.wakeUp.Set ();
				restart.WaitOne ();
				times++;
			}
		}
		private ThreadStart deleg;
		private Thread gThread;
		private System.Threading.EventWaitHandle wakeUp = new System.Threading.AutoResetEvent(false);
		private System.Threading.EventWaitHandle restart = new System.Threading.AutoResetEvent(false);
		private object forState = new object();
		private ThreadState internalState = ThreadState.Stopped;

		public ThreadStart NotMainLoopDeleg
		{
			set {
				lock (this.forState) { 
					if (this.internalState == ThreadState.Running)
						throw new NotSupportedException ("The Mainloop seems to be already running. Use a MainLoop delegate.");
				}
				deleg = value;
			}
		}
		
		public EventWaitHandle CallDelegInMainLoop (System.Threading.ThreadStart d)
		{
			exceptionHappened = null;
			EventWaitHandle wakeUpInMainLoop = new AutoResetEvent (false);
			lock (this.forState) { 
				if (this.internalState != ThreadState.Running)
					throw new NotSupportedException ("The Mainloop is not running yet.");
				lastHandle = wakeUpInMainLoop;
			}
			try {
				//WaitHandleDelegate w = new WaitHandleDelegate (d, wakeUpInMainLoop);
				
				return wakeUpInMainLoop;
			}
			finally {
				Gtk.Application.Invoke ( delegate { d (); wakeUpInMainLoop.Set (); });
			}
		}

		public ThreadState ThreadState
		{
			get { return gThread.ThreadState; }
		}

		public void JoinUntilSuspendedByMainLoop ()
		{
			bool wait = false;
			lock (forState)
			{
				if (this.internalState != ThreadState.Stopped)
					wait = true;
			}
			if (wait)
				wakeUp.WaitOne ();
		}

		public void Start ()
		{
			if (alreadyStarted)
			{
				lock (forState)
					this.internalState = ThreadState.Running;
				restart.Set ();
			}
			else
			{
				alreadyStarted = true;
				lock (forState)
					this.internalState = ThreadState.Running;
				this.gThread.Start ();
			}
		}

		public void Dispose ()
		{
			wakeUp.Close ();
			restart.Close ();
			gThread.Abort ();
		}
	}
}
