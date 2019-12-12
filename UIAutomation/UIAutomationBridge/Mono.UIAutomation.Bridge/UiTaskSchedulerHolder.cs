using System;
using System.Threading.Tasks;
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.Bridge
{
	public static class UiTaskSchedulerHolder
	{
		private const string EnvVarName = "MONO_UIA_UISYNCCONTEXT";
		private const string EnvVarTrue = "1";
		private const string EnvVarFlase = "0";
		private static TaskScheduler  _uiTaskScheduler = null;
		private static bool _warnMsgShown = false;

		public static TaskScheduler UiTaskScheduler
		{
			get
			{
				var envVarUseUiSyncContext = IsEnvVarUseUiSyncContext ();
				if (_uiTaskScheduler != null && envVarUseUiSyncContext)
					return _uiTaskScheduler;

				if (!_warnMsgShown) {
					var msg = envVarUseUiSyncContext
						? "[UiTaskSchedulerHolder]: UI SynchronizationContext is not set to deal with WinForms controls."
						: $"[UiTaskSchedulerHolder]: UI SynchronizationContext is not going to be used by means of '{EnvVarName}' environment viriable.";
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

		private static bool IsEnvVarUseUiSyncContext ()
		{
			var envVar = Environment.GetEnvironmentVariable (EnvVarName);
			if (envVar == null || envVar == "1")
				return true;
			if (envVar == "0")
				return false;
			
			var msg = $"[UiTaskSchedulerHolder]: Environment variable '{EnvVarName}' (currently set to '{envVar}')"
				+ $" may be set to '{EnvVarFlase}' or '{EnvVarTrue}' only. Unset variable is equal to '{EnvVarTrue}'.";
			Console.WriteLine (msg);
			throw new Exception (msg);
		}
	}
}