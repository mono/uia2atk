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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//      Andres Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public sealed class DynamicAdapterFactory
	{
#region Public Properties
		public static DynamicAdapterFactory Instance {
			get { return instance; }
		}
#endregion

#region Public Methods
		public Atk.Object GetAdapter (AutomationPeer peer)
		{
			if (peer == null)
				return null;

			// Create a list of all potential implementors that
			// will later be merged to have a list of implementors
			// with unique Atk interfaces.
			List<Type> implementors = new List<Type> ();

			// Do we have any explict implementors for this type?
			List<Type> explicitImpls;
			if (explicitImplementors.TryGetValue (peer.GetType (),
			                                      out explicitImpls)) {
				if (explicitImpls != null)
					implementors.AddRange (explicitImpls);
			}

			// Find implementors for the patterns that the peer
			// implements
			List<Type> potentialImpls = new List<Type> ();
			potentialImpls.AddRange (
				GetImplementorsForPeer (peer));

			// Create a reverse mapping between Atk interface and
			// implementor to ensure uniqueness.
			//
			// If there are collisions, favor the one with the most
			// Atk interfaces.
			Dictionary<Type, Type> atkInterfaces
				= new Dictionary<Type, Type> ();
			foreach (Type impl in potentialImpls) {
				Type [] ifaces = impl.GetInterfaces ();

				foreach (Type i in ifaces) {
					if (i.Namespace != "Atk")
						continue;

					if (atkInterfaces.ContainsKey (i)
					    && (ifaces.Length <= atkInterfaces [i].GetInterfaces ().Length))
						continue;

					atkInterfaces [i] = impl;
				}
			}

			// Add the distinct implementors
			implementors.AddRange (atkInterfaces.Select (x => x.Value).Distinct ());
			implementors.Sort ((a, b) => a.Name.CompareTo (b.Name));

			// Concat the type names together
			string typeName = implementors.Aggregate (String.Empty, (s, t) => s + t.Name)
			                  + "Adapter";

			// If we're not implementing anything, just
			// short-circuit the process
			if (implementors.Count () == 0)
				return new Adapter (peer);

			// See if we've created the type already
			Type dynamicType;
			if (!dynamicAdapters.TryGetValue (typeName,
			                                  out dynamicType)) {
				dynamicType = CreateDynamicType (
					typeName, implementors.ToArray ()
				);
			}

			if (dynamicType == null)
				return null;

			return (Atk.Object) Activator.CreateInstance (
				dynamicType, new object [] { peer }
			);
		}

		public Type [] GetImplementorsForPeer (AutomationPeer peer)
		{
			List<Type> implementors = new List<Type> ();

			foreach (PatternInterface pattern
			         in GetEnumValues<PatternInterface> ()) {
				var impl = peer.GetPattern (pattern);
				if (impl == null)
					continue;

				List<Type> impls;
				if (patternImplementors.TryGetValue (pattern,
				                                     out impls)) {
					implementors.AddRange (impls);
				}
			}

			return implementors.ToArray ();
		}

		public Type CreateDynamicType (string typeName,
		                               Type [] implementors)
		{
			Type [] atkInterfaces
				= implementors.SelectMany (i => i.GetInterfaces ())
			                      .Where (i => i.Namespace == "Atk")
			                      .Distinct ().ToArray ();

			TypeBuilder tb = moduleBuilder.DefineType (
				GENERATED_NAMESPACE + "." + typeName,
				TypeAttributes.Class | TypeAttributes.Public,
				typeof (Adapter), atkInterfaces
			);

			// Define a field for each of our implementors
			Dictionary<Type, FieldBuilder> fields
				= new Dictionary<Type, FieldBuilder> ();
			foreach (Type impl in implementors) {
				fields [impl] = tb.DefineField (
					CamelCaseType (impl), impl,
					FieldAttributes.Private
				);
			}

			// Default ctor
			ConstructorBuilder cb = tb.DefineConstructor (
				MethodAttributes.Public, CallingConventions.Standard,
				new Type [] { typeof (AutomationPeer) });

			ILGenerator ilgen = cb.GetILGenerator ();

			// Chain up our base ctor
			ilgen.Emit (OpCodes.Ldarg_0);
			ilgen.Emit (OpCodes.Ldarg_1);

			ilgen.Emit (OpCodes.Call, typeof (Adapter).GetConstructor (
				new Type [] { typeof (AutomationPeer) }));

			// Initialize all of our implementor fields
			foreach (KeyValuePair<Type, FieldBuilder> item
			         in fields) {
				// field = new TField (peer);
				ilgen.Emit (OpCodes.Ldarg_0);
				ilgen.Emit (OpCodes.Ldarg_1);

				ilgen.Emit (OpCodes.Newobj, item.Key.GetConstructor (
					new Type [] { typeof (AutomationPeer) }));

				ilgen.Emit (OpCodes.Stfld, item.Value);
			}

			ilgen.Emit (OpCodes.Ret);

			// Implement methods and properties for the interfaces
			// implemented by our implementors
			foreach (Type impl in implementors) {
				foreach (Type i in atkInterfaces) {
					Dictionary<MethodInfo, MethodBuilder> methodMapping
						= new Dictionary<MethodInfo, MethodBuilder> ();
					foreach (MethodInfo mi in i.GetMethods ()) {
						ParameterInfo [] paramInfos = mi.GetParameters ();
						Type [] paramTypes
							= paramInfos.Select (p => p.ParameterType)
						                    .ToArray ();

						// We're always overriding an Atk interface method
						MethodAttributes attrs = MethodAttributes.Public
							| MethodAttributes.HideBySig
							| MethodAttributes.Virtual
							| MethodAttributes.Final;

						// Set the right attrs for property methods
						if ((mi.Attributes & MethodAttributes.SpecialName) != 0)
							attrs |= MethodAttributes.SpecialName;

						MethodBuilder mb = tb.DefineMethod (
							mi.Name, attrs, mi.CallingConvention,
							mi.ReturnType, paramTypes
						);
						methodMapping [mi] = mb;

						// Preserve out semantics
						for (int j = 0; j < paramInfos.Length; j++) {
							mb.DefineParameter (j + 1,
								paramInfos [j].IsOut ? ParameterAttributes.Out
								                    : ParameterAttributes.None,
								paramInfos [j].Name
							);
						}

						ilgen = mb.GetILGenerator ();

						// this.<field>.<MethodName> (<params>)
						ilgen.Emit (OpCodes.Ldarg_0);
						ilgen.Emit (OpCodes.Ldfld, fields [impl]);

						for (int j = 0; j < paramTypes.Length; j++) {
							switch (j) {
							case 0:
								ilgen.Emit (OpCodes.Ldarg_1);
								break;
							case 1:
								ilgen.Emit (OpCodes.Ldarg_2);
								break;
							case 2:
								ilgen.Emit (OpCodes.Ldarg_3);
								break;
							default:
								ilgen.Emit (OpCodes.Ldarg_S, j + 1);
								break;
							}
						}

						ilgen.Emit (OpCodes.Callvirt, mi);
						ilgen.Emit (OpCodes.Ret);
					}

					// Create a property definition for
					// each of the properties in the
					// interface.
					foreach (PropertyInfo pi in i.GetProperties ()) {
						PropertyBuilder pb = tb.DefineProperty (
							pi.Name, pi.Attributes, pi.PropertyType, null
						);

						// Use our methodMapping to attach getters and setters.
						MethodInfo getMi = pi.GetGetMethod ();
						if (getMi != null && methodMapping.ContainsKey (getMi))
							pb.SetGetMethod (methodMapping [getMi]);

						MethodInfo setMi = pi.GetSetMethod ();
						if (setMi != null && methodMapping.ContainsKey (setMi))
							pb.SetSetMethod (methodMapping [setMi]);
					}
				}
			}

			return tb.CreateType ();
		}
#endregion

#region Private Methods
		private DynamicAdapterFactory ()
		{
			AssemblyName an = new AssemblyName ();
			an.Version = new Version (0, 0, 0, 1);
			an.Name = GENERATED_ASM_NAME;

			assemblyBuilder
				= AppDomain.CurrentDomain.DefineDynamicAssembly (
						an, AssemblyBuilderAccess.Run);

			moduleBuilder = assemblyBuilder.DefineDynamicModule (GENERATED_ASM_NAME);

			RegisterPatternImplementors ();
		}

		private void RegisterPatternImplementors ()
		{
			Assembly asm = typeof (DynamicAdapterFactory).Assembly;
			foreach (Type t in asm.GetTypes ()) {
				object [] attrs = t.GetCustomAttributes (
					typeof (ImplementsPatternAttribute),
					false);

				foreach (Attribute attr in attrs) {
					ImplementsPatternAttribute ipa
						= attr as ImplementsPatternAttribute;
					if (ipa == null)
						continue;

					if (ipa.ElementType != null) {
						if (!explicitImplementors.ContainsKey (ipa.ElementType))
							explicitImplementors [ipa.ElementType] = new List<Type> ();

						explicitImplementors [ipa.ElementType].Add (t);
						continue;
					}

					if (!patternImplementors.ContainsKey (ipa.Pattern)) {
						// Micro-optimization: Most
						// likely, we'll only have 1
						// implementor per pattern
						patternImplementors [ipa.Pattern]
							= new List<Type> (1);
					}

					patternImplementors [ipa.Pattern].Add (t);
				}
			}
		}

		// This is needed as Enum.GetValues isn't available in the
		// SL 2.0 API
		private T [] GetEnumValues<T> ()
		{
			Type enumType = typeof (T);
			if (!enumType.IsEnum)
				throw new ArgumentException (
					String.Format ("Type {0} is not an enum",
					               enumType));

			return enumType.GetFields ()
			               .Where (f => f.IsLiteral)
			               .Select (f => (T) f.GetValue (enumType))
			               .ToArray ();
		}

		// Assumed that type is already in PascalCase as per STYLE
		// guide.
		private string CamelCaseType (Type t)
		{
			string name = t.Name;
			return name.Length > 0 ? Char.ToLower (name [0]) + name.Substring (1)
			                       : String.Empty;
		}
#endregion

#region Private Fields
		private static DynamicAdapterFactory instance
			= new DynamicAdapterFactory ();

		private Dictionary<PatternInterface, List<Type>> patternImplementors
			= new Dictionary<PatternInterface, List<Type>> ();

		private Dictionary<Type, List<Type>> explicitImplementors
			= new Dictionary<Type, List<Type>> ();

		// Maps generated type name to the generated adapter class
		private Dictionary<string, Type> dynamicAdapters
			= new Dictionary<string, Type> ();

		private ModuleBuilder moduleBuilder;
		private AssemblyBuilder assemblyBuilder;

		private const char TYPE_SEPARATOR = '_';
		private const string GENERATED_ASM_NAME = "DynamicAtkTypes";
		private const string GENERATED_NAMESPACE = "Moonlight.AtkBridge.DynamicAdapters";
#endregion
	}
}
