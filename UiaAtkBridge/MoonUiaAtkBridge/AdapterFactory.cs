
using System;

using System.Collections.Generic;

using System.Linq;

using System.Reflection;
using System.Reflection.Emit;

using System.Windows.Automation.Peers;

namespace MoonUiaAtkBridge
{
	public class AdapterFactory
	{
		internal const char TYPE_SEPARATOR = '_';

		internal static PatternInterface [] allPatterns;

		static AdapterFactory ()
		{
			allPatterns = EnumHelper.GetValues <PatternInterface> ();
		}
		
		static List<Type> GetAtkInterfacesForPeer (AutomationPeer peer) {
			List<Type> atkTypes = new List<Type> ();
			
			List<PatternInterface> patterns = new List<PatternInterface> ();
			
			foreach (PatternInterface pattern in allPatterns) {
				if (peer.GetPattern (pattern) != null)
					patterns.Add (pattern);
			}
			
			if (patterns.Contains (PatternInterface.Value)) {
				atkTypes.Add (typeof (Atk.Text));
			} //TODO: else if ...
			
			return atkTypes;
		}

		internal Type GetDynamicType (AutomationPeer thePeer)
		{
			List<Type> atkInterfaces = GetAtkInterfacesForPeer (thePeer);
			
			const string dynamicTypesName = "DynamicAtkTypes";
			AssemblyName an = new AssemblyName ();
			an.Version = new Version (1, 0, 0, 0);
			an.Name = dynamicTypesName;
			
			// Define a dynamic assembly
			AssemblyBuilder ab = System.AppDomain.CurrentDomain.DefineDynamicAssembly (an, AssemblyBuilderAccess.Run);
			ModuleBuilder modBuilder = ab.DefineDynamicModule (dynamicTypesName);

			string typeName = String.Empty;
			foreach (Type t in atkInterfaces)
				typeName += TYPE_SEPARATOR + t.Name;
			
			TypeBuilder tb = 
				modBuilder.DefineType (MethodInfo.GetCurrentMethod().DeclaringType.Namespace + "." +
				                       dynamicTypesName + "." + typeName,
				                       TypeAttributes.Class | TypeAttributes.Public,
				                       typeof (Adapter), atkInterfaces.ToArray());

			Dictionary <Type, FieldBuilder> fields = new Dictionary<Type, FieldBuilder> ();
			foreach (Type i in atkInterfaces)
				fields.Add (i, tb.DefineField (i.Name.ToLower (), i, FieldAttributes.Private));

			ConstructorBuilder cb = tb.DefineConstructor (MethodAttributes.Public, CallingConventions.Standard, 
			                                              new Type [] { typeof (Adapter), typeof (AutomationPeer) } );

			ILGenerator ilgen = cb.GetILGenerator ();
			
			// Load "this"
			ilgen.Emit (OpCodes.Ldarg_0);
			
			// Call the base constructor (no args)
			ilgen.Emit (OpCodes.Call, typeof (Adapter).GetConstructor (new Type [] { typeof (AutomationPeer) }));
			
			// Emit return opcode
			ilgen.Emit (OpCodes.Ret);

			foreach (Type i in atkInterfaces) {
				foreach (MethodInfo method in i.GetMethods ()) {
					List<Type> paramTypes = new List<Type> ();
					foreach (ParameterInfo param in method.GetParameters ())
						paramTypes.Add (param.ParameterType);
					MethodBuilder mb = tb.DefineMethod (method.Name, 
					                                    MethodAttributes.Public, 
					                                    CallingConventions.Standard,
					                                    method.ReturnType, paramTypes.ToArray ());
					
					// Get the IL generator for the method
					ilgen = mb.GetILGenerator ();
					
					// Load "this"
					ilgen.Emit(OpCodes.Ldarg_0);
					
					// Load field
					ilgen.Emit (OpCodes.Ldfld, fields [i]);

					//TODO: call the method "method" on this field, with the same params

				}
				//TODO: properties
			}

			return tb.CreateType ();

		}

		internal class EnumHelper {
			
			public static T [] GetValues <T> ()
			{
				Type enumType = typeof (T);
	
				if (!enumType.IsEnum)
					throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
	
				List <T> values = new List <T> ();
	
				var fields = from field in enumType.GetFields ()
				  where field.IsLiteral
				  select field;
	
				foreach (FieldInfo field in fields)
				{
					object value = field.GetValue (enumType);
					values.Add ((T)value);
				}
				
				return values.ToArray ();
			}
		}
	}

}
