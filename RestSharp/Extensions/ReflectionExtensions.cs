#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
#if NETFX_CORE
using System.Reflection.RuntimeExtensions;
#endif

namespace RestSharp.Extensions
{
	/// <summary>
	/// Reflection extensions
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Retrieve an attribute from a member (property)
		/// </summary>
		/// <typeparam name="T">Type of attribute to retrieve</typeparam>
		/// <param name="prop">Member to retrieve attribute from</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this MemberInfo prop) where T : Attribute {
#if NETFX_CORE
            return prop.GetCustomAttribute<T>();
#else
			return Attribute.GetCustomAttribute(prop, typeof(T)) as T;
#endif
		}


        public static  bool  IsGenericType(this Type item)
        {
#if NETFX_CORE
            return item.GetTypeInfo().IsGenericType;
#else
            return item.IsGenericType;
#endif
        }

        public static bool IsPrimitive(this Type item)
        {
#if NETFX_CORE
            return item.GetTypeInfo().IsPrimitive;
#else
            return item.IsPrimitive;
#endif
        }

        public static bool IsPublic(this Type item)
        {
#if NETFX_CORE
            return item.GetTypeInfo().IsPublic;
#else
            return item.IsPublic;
#endif
        }

        public static bool IsEnum(this Type item)
        {
#if NETFX_CORE
            return item.GetTypeInfo().IsEnum;
#else
            return item.IsEnum;
#endif
        }


        /// <summary>
		/// Retrieve an attribute from a type
		/// </summary>
		/// <typeparam name="T">Type of attribute to retrieve</typeparam>
		/// <param name="type">Type to retrieve attribute from</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Type type) where T : Attribute {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttribute<T>();
            //return Attribute. GetCustomAttribute(type, typeof(T)) as T;
#else
			return Attribute.GetCustomAttribute(type, typeof(T)) as T;
#endif
		}

        public static Type BaseType(this System.Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }


#if NETFX_CORE
        public static Type[] GetGenericArguments(this Type t)
        {
            return t.GetTypeInfo().GenericTypeArguments;
        }
#endif 

		/// <summary>
		/// Checks a type to see if it derives from a raw generic (e.g. List[[]])
		/// </summary>
		/// <param name="toCheck"></param>
		/// <param name="generic"></param>
		/// <returns></returns>
		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic) {
			while (toCheck != typeof(object)) {
				var cur = toCheck.IsGenericType() ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur) {
					return true;
				}
				toCheck = toCheck.BaseType();
			}
			return false;
		}

		public static object ChangeType(this object source, Type newType)
		{
#if FRAMEWORK
			return Convert.ChangeType(source, newType);
#else
			return Convert.ChangeType(source, newType, null);
#endif
		}

		public static object ChangeType(this object source, Type newType, CultureInfo culture)
		{
#if FRAMEWORK
			return Convert.ChangeType(source, newType, culture);
#else
			return Convert.ChangeType(source, newType, null);
#endif
		}

		/// <summary>
		/// Find a value from a System.Enum by trying several possible variants
		/// of the string value of the enum.
		/// </summary>
		/// <param name="type">Type of enum</typeparam>
		/// <param name="value">Value for which to search</param>
		/// <param name="culture">The culture used to calculate the name variants</param>
		/// <returns></returns>
		public static object FindEnumValue(this Type type, string value, CultureInfo culture)
		{
#if FRAMEWORK
			return Enum.GetValues(type)
				.Cast<Enum>()
				.First(v => v.ToString().GetNameVariants(culture).Contains(value, StringComparer.Create(culture, true)));
#else
			return Enum.Parse(type, value, true);
#endif
		}


#if NETFX_CORE
        public static IEnumerable<PropertyInfo> GetProperties(this Type t)
        {
            return t.GetRuntimeProperties();
        }

        //FROM Jeff Wilcox https://gist.github.com/2432351
        /// <summary>
        /// Determines whether the specified object is an instance of the current Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="o">The object to compare with the current type.</param>
        /// <returns>true if the current Type is in the inheritance hierarchy of the 
        /// object represented by o, or if the current Type is an interface that o 
        /// supports. false if neither of these conditions is the case, or if o is 
        /// null, or if the current Type is an open generic type (that is, 
        /// ContainsGenericParameters returns true).</returns>
        public static bool IsInstanceOfType(this Type type, object o)
        {
            return o != null && type.IsAssignableFrom(o.GetType());
        }


        public  static bool ImplementInterface(this Type type, Type ifaceType)
        {
            while (type != null)
            {
                Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray(); //  .GetInterfaces();
                if (interfaces != null)
                {
                    for (int i = 0; i < interfaces.Length; i++)
                    {
                        if (interfaces[i] == ifaceType || (interfaces[i] != null && interfaces[i].ImplementInterface(ifaceType)))
                        {
                            return true;
                        }
                    }
                }
                type = type.GetTypeInfo().BaseType;
                // type = type.BaseType;
            }
            return false;
        }


        public static bool IsAssignableFrom(this Type type, Type c)
        {
            if (c == null)
            {
                return false;
            }
            if (type == c)
            {
                return true;
            }


            //RuntimeType runtimeType = type.UnderlyingSystemType as RuntimeType;
            //if (runtimeType != null)
            //{
            //    return runtimeType.IsAssignableFrom(c);
            //}


            //if (c.IsSubclassOf(type))
            if (c.GetTypeInfo().IsSubclassOf(c))
            {
                return true;
            }


            //if (type.IsInterface)
            if (type.GetTypeInfo().IsInterface)
            {
                return c.ImplementInterface(type);
            }


            if (type.IsGenericParameter)
            {
                Type[] genericParameterConstraints = type.GetTypeInfo().GetGenericParameterConstraints();
                for (int i = 0; i < genericParameterConstraints.Length; i++)
                {
                    if (!genericParameterConstraints[i].IsAssignableFrom(c))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
#endif
	}
}
