using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Reflection;

namespace ReflectionTest {
	public partial class Page : UserControl {
		public Page ()
		{
			InitializeComponent ();
		}

		static string typeToCheck = null;

		private void Button_Click (object sender, RoutedEventArgs e)
		{
			try {
				string AutomationBridgeType = "Moonlight.AtkBridge.AutomationBridge, MoonAtkBridge";
				Button butt = sender as Button;
				Type typeObtained = null;

				if (((string)(butt.Name)).EndsWith ("1"))
					typeToCheck = "ReflectionTest.Page";
				else if (((string)(butt.Name)).EndsWith ("2"))
					typeToCheck = "NonExistant.NoType";
				else if (((string)(butt.Name)).EndsWith ("3"))
					typeToCheck = "System.Collections.Hashtable, mscorlib";
				else if (((string)(butt.Name)).EndsWith ("4"))
					typeToCheck = AutomationBridgeType;
				else if (((string)(butt.Name)).EndsWith ("5")) {
					System.Reflection.Assembly ass = null;
					try {
						ass = Assembly.Load ("MoonAtkBridge");
						if (ass != null)
							((Button) sender).Content = "it worked";
						else
							((Button) sender).Content = "it did not work";
					} catch {
						((Button) sender).Content = "exception";
					}
					return;
				}
	
				if (typeToCheck != null) {
					typeObtained = Type.GetType (typeToCheck);
					if (typeObtained == null)
						((Button) sender).Content = "Type.GetType returned null";
					else {

						ConstructorInfo ctor = null;
						MethodInfo method = null;
						if (typeToCheck != AutomationBridgeType)
							ctor = typeObtained.GetConstructor (new Type [0]);
						else {
							method = typeObtained.GetMethod ("CreateAutomationBridge",
							                                 System.Reflection.BindingFlags.NonPublic |
							                                 System.Reflection.BindingFlags.Static);
						}

						if (method == null && ctor == null)
							((Button) sender).Content = "member of type returned null";
						else {
							try {
								if (method != null)
									method.Invoke (null, null);
								else
									ctor.Invoke (new object [0]);
								((Button) sender).Content = "it worked";
							} catch (MethodAccessException) {
								((Button) sender).Content = "MAEx";
							}
						}
					}
				}
			} catch (Exception ex) {
				((Button) sender).Content = "global EX: " + ex.ToString ();
			}
		}
	}
}
