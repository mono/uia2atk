// FIXME: If you comment the other classes in this project and make this one public, you'll get a deadlock on this, why?

// NOTE: the following is the output I get when killing the process with the -SIGQUIT option:

//NUnit version 2.4.7
//Copyright (C) 2002-2007 Charlie Poole.
//Copyright (C) 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov.
//Copyright (C) 2000-2002 Philip Craig.
//All Rights Reserved.
//
//Runtime Environment - 
//   OS Version: Unix 2.6.25.18
//  CLR Version: 2.0.50727.1433 ( Mono 2.3 )
//
//Selected test: UiaAtkBridgeTest.BridgeTests.OpenFileDialog
//***** UiaAtkBridgeTest.BridgeTests.OpenFileDialog
//Form added!
//AutomationBridge: Unhandled control: ControlType.Window
//
//Tests run: 1, Failures: 0, Not run: 0, Time: 4.956 seconds
//
//
//Full thread dump:
//
//"" tid=0x0xb64f1b90 this=0x0x59c578:
//
//"" tid=0x0xb7cd36f0 this=0x0x2fed8:
//  at (wrapper managed-to-native) System.AppDomain.InternalUnload (int) <0x00004>
//  at (wrapper managed-to-native) System.AppDomain.InternalUnload (int) <0xffffffff>
//  at System.AppDomain.Unload (System.AppDomain) <0x00024>
//  at NUnit.Util.DomainManager.Unload (System.AppDomain) <0x00053>
//  at NUnit.Util.TestDomain.Unload () <0x00032>
//  at NUnit.ConsoleRunner.ConsoleUi.Execute (NUnit.ConsoleRunner.ConsoleOptions) <0x009f4>
//  at NUnit.ConsoleRunner.Runner.Main (string[]) <0x0038e>
//  at NUnit.ConsoleRunner.Class1.Main (string[]) <0x00010>
//  at (wrapper runtime-invoke) NUnit.ConsoleRunner.Class1.runtime_invoke_int_object (object,intptr,intptr,intptr) <0xffffffff>
//
//"<threadpool thread>" tid=0x0xb38ffb90 this=0x0x59cd48:
//
//"<threadpool thread>" tid=0x0xb37feb90 this=0x0x59cc80:
//
//"" tid=0x0xb4fffb90 this=0x0x142d48:
//  at (wrapper managed-to-native) GLib.MainLoop.g_main_loop_run (intptr) <0x00004>
//  at (wrapper managed-to-native) GLib.MainLoop.g_main_loop_run (intptr) <0xffffffff>
//  at GLib.MainLoop.Run () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/gtk-sharp/glib/MainLoop.cs:59
//  at UiaAtkBridge.Monitor.GLibMainLoopThread () [0x0000b] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/uia2atk/UiaAtkBridge/UiaAtkBridge/Monitor.cs:128
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//
//"Timer-Scheduler" tid=0x0xb6c69b90 this=0x0x142a28:
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0x00004>
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0xffffffff>
//  at System.Threading.WaitHandle.WaitOne (int,bool) [0x00020] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/WaitHandle.cs:315
//  at System.Threading.Timer.SchedulerThread () [0x0021b] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/Timer.cs:132
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>

using System;

using NUnit.Framework;

using System.Windows.Forms;

namespace UiaAtkBridgeTest
{

	//[TestFixture]
	public class FigureOutDeadlock
	{

		[TestFixtureSetUp]
		public void FigureOutDeadlockInit () 
		{
			//see clarification about this line at the end of this file:
			new Form ();
			
			BridgeTester.InitializeA11y ();
		}

		[Test]
		public void OpenFileDialogCausesDeadlock ()
		{
			BridgeTests.OpenFileDialogStatic ();
		}

		[TestFixtureTearDown]
		public void TearDown ()
		{
			BridgeTests.BridgeTearDown ();
		}
	}
}


//if you comment the "new Form();" line, you'll get a different deadlock:
//Full thread dump:
//
//"" tid=0x0xb62feb90 this=0x0x141640:
//  at (wrapper managed-to-native) GLib.MainLoop.g_main_loop_run (intptr) <0x00004>
//  at (wrapper managed-to-native) GLib.MainLoop.g_main_loop_run (intptr) <0xffffffff>
//  at GLib.MainLoop.Run () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/gtk-sharp/glib/MainLoop.cs:59
//  at UiaAtkBridge.Monitor.GLibMainLoopThread () [0x0000b] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/uia2atk/UiaAtkBridge/UiaAtkBridge/Monitor.cs:128
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//
//"<threadpool thread>" tid=0x0xb4f13b90 this=0x0x1413e8:
//
//"<threadpool thread>" tid=0x0xb5014b90 this=0x0x1414b0:
//
//"Timer-Scheduler" tid=0x0xb6bffb90 this=0x0x141d48:
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0x00004>
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0xffffffff>
//  at System.Threading.WaitHandle.WaitOne (int,bool) [0x00020] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/WaitHandle.cs:315
//  at System.Threading.Timer.SchedulerThread () [0x0021b] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/Timer.cs:132
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//
//"EventPumpThread" tid=0x0xb63ffb90 this=0x0x141708:
//  at (wrapper managed-to-native) System.Threading.Monitor.Monitor_wait (object,int) <0x00004>
//  at (wrapper managed-to-native) System.Threading.Monitor.Monitor_wait (object,int) <0xffffffff>
//  at System.Threading.Monitor.Wait (object) [0x00027] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/Monitor.cs:179
//  at NUnit.Core.EventPump.PumpThreadProc () <0x000d8>
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//
//"Timer-Scheduler" tid=0x0xb6e9ab90 this=0x0x141000:
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0x00004>
//  at (wrapper managed-to-native) System.Threading.WaitHandle.WaitOne_internal (intptr,int,bool) <0xffffffff>
//  at System.Threading.WaitHandle.WaitOne (int,bool) [0x00020] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/WaitHandle.cs:315
//  at System.Threading.Timer.SchedulerThread () [0x0021b] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Threading/Timer.cs:132
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//
//"" tid=0x0xb7def6f0 this=0x0x2fed8:
//  at (wrapper managed-to-native) System.Windows.Forms.X11DesktopColors.gtk_init_check (intptr,intptr) <0x00004>
//  at (wrapper managed-to-native) System.Windows.Forms.X11DesktopColors.gtk_init_check (intptr,intptr) <0xffffffff>
//  at System.Windows.Forms.X11DesktopColors.GtkInit () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/X11DesktopColors.cs:152
//  at System.Windows.Forms.X11DesktopColors..cctor () [0x0001d] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/X11DesktopColors.cs:101
//  at (wrapper runtime-invoke) object.runtime_invoke_void (object,intptr,intptr,intptr) <0xffffffff>
//  at System.Windows.Forms.XplatUIX11..ctor () [0x000c9] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/XplatUIX11.cs:279
//  at System.Windows.Forms.XplatUIX11..ctor () [0x00077] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/XplatUIX11.cs:269
//  at System.Windows.Forms.XplatUIX11.GetInstance () [0x00018] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/XplatUIX11.cs:292
//  at System.Windows.Forms.XplatUI..cctor () [0x00099] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/XplatUI.cs:103
//  at (wrapper runtime-invoke) object.runtime_invoke_void (object,intptr,intptr,intptr) <0xffffffff>
//  at System.Windows.Forms.Theme.get_MenuAccessKeysUnderlined () [0x00006] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/Theme.cs:491
//  at System.Windows.Forms.Theme.get_MenuAccessKeysUnderlined () [0x00006] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/Theme.cs:491
//  at System.Windows.Forms.SystemInformation.get_MenuAccessKeysUnderlined () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/SystemInformation.cs:346
//  at System.Windows.Forms.Control..ctor () [0x000e3] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/Control.cs:1060
//  at (wrapper remoting-invoke-with-check) System.Windows.Forms.Control..ctor () <0xffffffff>
//  at System.Windows.Forms.WindowsFormsSynchronizationContext..cctor () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at (wrapper runtime-invoke) object.runtime_invoke_void (object,intptr,intptr,intptr) <0xffffffff>
//  at System.Windows.Forms.Control..ctor () [0x001fa] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/Control.cs:1094
//  at System.Windows.Forms.Control..ctor () [0x00014] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/Control.cs:1030
//  at System.Windows.Forms.ScrollableControl..ctor () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at System.Windows.Forms.ContainerControl..ctor () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at System.Windows.Forms.Form..ctor () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at System.Windows.Forms.CommonDialog/DialogForm..ctor (System.Windows.Forms.CommonDialog) [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at (wrapper remoting-invoke-with-check) System.Windows.Forms.CommonDialog/DialogForm..ctor (System.Windows.Forms.CommonDialog) <0xffffffff>
//  at System.Windows.Forms.FileDialog..ctor () [0x00075] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/Managed.Windows.Forms/System.Windows.Forms/FileDialog.cs:133
//  at System.Windows.Forms.OpenFileDialog..ctor () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/build/common/Consts.cs:1
//  at (wrapper remoting-invoke-with-check) System.Windows.Forms.OpenFileDialog..ctor () <0xffffffff>
//  at UiaAtkBridgeTest.BridgeTests.OpenFileDialogStatic () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/uia2atk/UiaAtkBridge/Test/UiaAtkBridgeTest/AssemblyInfo.cs:1
//  at UiaAtkBridgeTest.FigureOutDeadlock.OpenFileDialogCausesDeadlock () [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/a11y/uia2atk/UiaAtkBridge/Test/UiaAtkBridgeTest/AssemblyInfo.cs:1
//  at (wrapper runtime-invoke) object.runtime_invoke_void__this__ (object,intptr,intptr,intptr) <0xffffffff>
//  at (wrapper managed-to-native) System.Reflection.MonoMethod.InternalInvoke (object,object[],System.Exception&) <0x00004>
//  at (wrapper managed-to-native) System.Reflection.MonoMethod.InternalInvoke (object,object[],System.Exception&) <0xffffffff>
//  at System.Reflection.MonoMethod.Invoke (object,System.Reflection.BindingFlags,System.Reflection.Binder,object[],System.Globalization.CultureInfo) [0x00057] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Reflection/MonoMethod.cs:159
//  at System.Reflection.MethodBase.Invoke (object,object[]) [0x00000] in /home/knocte/Documents/iDocs/Proyectos/mono/trunk/mcs/class/corlib/System.Reflection/MethodBase.cs:111
//  at NUnit.Core.Reflect.InvokeMethod (System.Reflection.MethodInfo,object,object[]) <0x00030>
//  at NUnit.Core.Reflect.InvokeMethod (System.Reflection.MethodInfo,object) <0x00015>
//  at NUnit.Core.TestMethod.RunTestMethod (NUnit.Core.TestCaseResult) <0x00018>
//  at NUnit.Core.TestMethod.doTestCase (NUnit.Core.TestCaseResult) <0x00020>
//  at NUnit.Core.TestMethod.doRun (NUnit.Core.TestCaseResult) <0x00076>
//  at NUnit.Core.TestMethod.Run (NUnit.Core.TestCaseResult) <0x0011f>
//  at NUnit.Core.NUnitTestMethod.Run (NUnit.Core.TestCaseResult) <0x00015>
//  at NUnit.Core.TestCase.Run (NUnit.Core.EventListener) <0x000fd>
//  at NUnit.Core.TestCase.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x00016>
//  at NUnit.Core.TestSuite.RunAllTests (NUnit.Core.TestSuiteResult,NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x000e0>
//  at NUnit.Core.TestSuite.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x001d9>
//  at NUnit.Core.TestFixture.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x00060>
//  at NUnit.Core.TestSuite.RunAllTests (NUnit.Core.TestSuiteResult,NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x000e0>
//  at NUnit.Core.TestSuite.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x001d9>
//  at NUnit.Core.TestSuite.RunAllTests (NUnit.Core.TestSuiteResult,NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x000e0>
//  at NUnit.Core.TestSuite.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x001d9>
//  at NUnit.Core.SimpleTestRunner.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x00098>
//  at NUnit.Core.ProxyTestRunner.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x00023>
//  at NUnit.Core.RemoteTestRunner.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x000a1>
//  at (wrapper xdomain-dispatch) NUnit.Core.RemoteTestRunner.Run (object,byte[]&,byte[]&) <0xffffffff>
//  at (wrapper xdomain-invoke) NUnit.Core.RemoteTestRunner.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0xffffffff>
//  at NUnit.Core.ProxyTestRunner.Run (NUnit.Core.EventListener,NUnit.Core.ITestFilter) <0x00023>
//  at NUnit.ConsoleRunner.ConsoleUi.Execute (NUnit.ConsoleRunner.ConsoleOptions) <0x006a8>
//  at NUnit.ConsoleRunner.Runner.Main (string[]) <0x0038e>
//  at NUnit.ConsoleRunner.Class1.Main (string[]) <0x00010>
//  at (wrapper runtime-invoke) NUnit.ConsoleRunner.Class1.runtime_invoke_int_object (object,intptr,intptr,intptr) <0xffffffff>
