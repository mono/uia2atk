
using System;

using System.Reflection;

using System.Windows.Automation.Peers;

namespace MoonUiaAtkBridge
{
	
	public class Adapter : Atk.Object
	{
		public Adapter (AutomationPeer peer)
		{
			Type myType = this.GetType ();
			string [] atkInterfaces = myType.Name.Split (AdapterFactory.TYPE_SEPARATOR);
			
			foreach (string atkInterface in atkInterfaces) {
				Type implementor = Type.GetType (atkInterface + "Implementor");
				FieldInfo field = myType.GetField (atkInterface.ToLower ());
				field.SetValue (this, Activator.CreateInstance (implementor, new object [] { this, peer }));
			}
		}
	}
}
