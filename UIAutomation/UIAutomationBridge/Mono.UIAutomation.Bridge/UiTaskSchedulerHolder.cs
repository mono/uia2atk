using System;
using System.Threading.Tasks;
using Mono.UIAutomation.Helpers;
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.Bridge
{
	public static class UiTaskSchedulerHolder
	{
		private static TaskScheduler  _uiTaskScheduler = null;
		private static bool _warnMsgShown = false;

		public static TaskScheduler UiTaskScheduler
		{
			get
			{
				var envVarUseUiSyncContext = EnvironmentVaribles.MONO_UIA_UISYNCCONTEXT;
				if (_uiTaskScheduler != null && envVarUseUiSyncContext)
					return _uiTaskScheduler;

				if (!_warnMsgShown) {
					var msg = envVarUseUiSyncContext
						? $"[UiTaskSchedulerHolder]: UI SynchronizationContext is not set to deal with WinForms controls."
						: $"[UiTaskSchedulerHolder]: UI SynchronizationContext is not going to be used by means of 'MONO_UIA_UISYNCCONTEXT' environment viriable.";
					Console.WriteLine (msg);
					_warnMsgShown = true;
				}

				return null;
			}
		}

		// This method must be called from the UI-thread. Thus use them in the `FormListener`.
		// WARNING: This is not 100% good aproach with respect to multiple UI-thread case.
		public static void InitOnceFromCurrentSyncContext ()
		{
			if (_uiTaskScheduler == null)
			 	_uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext ();
		}
	}
}