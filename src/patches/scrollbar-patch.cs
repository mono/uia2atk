Index: class/Managed.Windows.Forms/System.Windows.Forms/ScrollBar.cs
===================================================================
--- class/Managed.Windows.Forms/System.Windows.Forms/ScrollBar.cs	(revision 106688)
+++ class/Managed.Windows.Forms/System.Windows.Forms/ScrollBar.cs	(working copy)
@@ -80,6 +80,16 @@
 		bool thumb_entered;
 		#endregion	// Local Variables
 
+		#region UIA Events	
+#if NET_2_0
+    		// There are used by externally by UIA framework
+    		internal event EventHandler LargeIncrementEvent;
+    		internal event EventHandler LargeDecrementEvent;
+    		internal event EventHandler SmallIncrementEvent;
+    		internal event EventHandler SmallDecrementEvent;
+#endif
+		#endregion
+
 		private enum TimerType
 		{
 			HoldButton,
@@ -788,6 +798,11 @@
 			event_args = new ScrollEventArgs (ScrollEventType.EndScroll, Value);
 			OnScroll (event_args);
     			Value = event_args.NewValue;
+		
+#if NET_2_0
+    			if (LargeIncrementEvent != null)
+	    			LargeIncrementEvent (this, null);
+#endif
     		}
 
     		private void LargeDecrement ()
@@ -802,6 +817,11 @@
 			event_args = new ScrollEventArgs (ScrollEventType.EndScroll, Value);
 			OnScroll (event_args);
     			Value = event_args.NewValue;
+    			
+#if NET_2_0
+    			if (LargeDecrementEvent != null)
+	    			LargeDecrementEvent (this, null);
+#endif
     		}
 
     		private void OnResizeSB (Object o, EventArgs e)
@@ -1296,6 +1316,11 @@
 			event_args = new ScrollEventArgs (ScrollEventType.EndScroll, Value);
 			OnScroll (event_args);
 			Value = event_args.NewValue;
+			
+#if NET_2_0
+    			if (SmallIncrementEvent != null)
+	    			SmallIncrementEvent (this, null);
+#endif
     		}
 
     		private void SmallDecrement ()
@@ -1310,6 +1335,11 @@
 			event_args = new ScrollEventArgs (ScrollEventType.EndScroll, Value);
 			OnScroll (event_args);
 			Value = event_args.NewValue;
+			
+#if NET_2_0
+    			if (SmallDecrementEvent != null)
+	    			SmallDecrementEvent (this, null);
+#endif			
     		}
 
     		private void SetHoldButtonClickTimer ()
