using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Linq.Expressions;

namespace Andamio
{
    #region ReflectionProperty
    public class ReflectionProperty
    {
        #region Constructors
        private ReflectionProperty()
        {
        }
        internal ReflectionProperty(object instance, PropertyInfo property)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (property == null) throw new ArgumentNullException("property");
            PropertyMetadata = property;
            _instance = instance;
        }

        #endregion

        #region Metadata
        public PropertyInfo PropertyMetadata { get; private set; }
        private readonly object _instance;
        #endregion

        #region Value
        public object Value
        {
            get { return PropertyMetadata.GetValue(_instance); }
            set { PropertyMetadata.SetValue(_instance, value); }
        }

        #endregion
    }

    #endregion


    /// <summary>
    /// Extension class for Reflection related functionality.
    /// </summary>
    public static class ReflectionExtensions
    {
        #region Constructors
        private static readonly Dictionary<string, Type> FriendlyTypeNames = new Dictionary<string, Type>();
        static ReflectionExtensions()
        {
            FriendlyTypeNames["object"] = typeof(System.Object);
            FriendlyTypeNames["string"] = typeof(System.String);
            FriendlyTypeNames["bool"] = typeof(System.Boolean);
            FriendlyTypeNames["byte"] = typeof(System.Byte);
            FriendlyTypeNames["char"] = typeof(System.Char);
            FriendlyTypeNames["decimal"] = typeof(System.Decimal);
            FriendlyTypeNames["double"] = typeof(System.Double);
            FriendlyTypeNames["short"] = typeof(System.Int16);
            FriendlyTypeNames["int"] = typeof(System.Int32);
            FriendlyTypeNames["long"] = typeof(System.Int64);
            FriendlyTypeNames["sbyte"] = typeof(System.SByte);
            FriendlyTypeNames["float"] = typeof(System.Single);
            FriendlyTypeNames["ushort"] = typeof(System.UInt16);
            FriendlyTypeNames["uint"] = typeof(System.UInt32);
            FriendlyTypeNames["ulong"] = typeof(System.UInt64);
            FriendlyTypeNames["void"] = typeof(void);
            FriendlyTypeNames["datetime"] = typeof(System.DateTime);
            FriendlyTypeNames["guid"] = typeof(System.Guid);
            FriendlyTypeNames["OleDateTime"] = typeof(Andamio.OleDateTime);
            FriendlyTypeNames["Int"] = typeof(System.Int32);
            FriendlyTypeNames["Integer"] = typeof(System.Int32);

            FriendlyTypeNames["bool?"] = typeof(Nullable<System.Boolean>);
            FriendlyTypeNames["byte?"] = typeof(Nullable<System.Byte>);
            FriendlyTypeNames["char?"] = typeof(Nullable<System.Char>);
            FriendlyTypeNames["decimal?"] = typeof(Nullable<System.Decimal>);
            FriendlyTypeNames["double?"] = typeof(Nullable<System.Double>);
            FriendlyTypeNames["short?"] = typeof(Nullable<System.Int16>);
            FriendlyTypeNames["int?"] = typeof(Nullable<System.Int32>);
            FriendlyTypeNames["long?"] = typeof(Nullable<System.Int64>);
            FriendlyTypeNames["sbyte?"] = typeof(Nullable<System.SByte>);
            FriendlyTypeNames["float?"] = typeof(Nullable<System.Single>);
            FriendlyTypeNames["ushort?"] = typeof(Nullable<System.UInt16>);
            FriendlyTypeNames["uint?"] = typeof(Nullable<System.UInt32>);
            FriendlyTypeNames["ulong?"] = typeof(Nullable<System.UInt64>);
            FriendlyTypeNames["datetime?"] = typeof(Nullable<System.DateTime>);
            FriendlyTypeNames["guid?"] = typeof(Nullable<System.Guid>);
            FriendlyTypeNames["OleDateTime?"] = typeof(Nullable<Andamio.OleDateTime>);

            FriendlyTypeNames["Boolean?"] = typeof(Nullable<System.Boolean>);
            FriendlyTypeNames["Byte?"] = typeof(Nullable<System.Byte>);
            FriendlyTypeNames["Char?"] = typeof(Nullable<System.Char>);
            FriendlyTypeNames["Decimal?"] = typeof(Nullable<System.Decimal>);
            FriendlyTypeNames["Double?"] = typeof(Nullable<System.Double>);
            FriendlyTypeNames["Int?"] = typeof(System.Int32);
            FriendlyTypeNames["Integer?"] = typeof(System.Int32);            
            FriendlyTypeNames["SByte?"] = typeof(Nullable<System.SByte>);
            FriendlyTypeNames["DateTime?"] = typeof(Nullable<System.DateTime>);
            FriendlyTypeNames["Guid?"] = typeof(Nullable<System.Guid>);
        }

        #endregion

        #region Attributes
        /// <summary>
        /// Gets the custom attribute of type T assigned to the specified member.
        /// </summary>
        /// <typeparam name="T">Attribute type to retrieve.</typeparam>
        /// <param name="memberInfo">The class member decorated with the custom attribute.</param>
        /// <returns>The attribute assigned to the class member.</returns>
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return Attribute.GetCustomAttribute(memberInfo, typeof(T)) as T;
        }

        /// <summary>
        /// Gets the custom attribute of type T assigned to the specified instance.
        /// </summary>
        /// <typeparam name="T">Attribute type to retrieve.</typeparam>
        /// <param name="instance">The instance decorated with the custom attribute.</param>
        /// <returns>The attribute assigned to the instance.</returns>
        public static T GetCustomAttribute<T>(this object instance) where T : Attribute
        {
            var attributes = GetCustomAttributes<T>(instance);
            if (attributes != null && attributes.Any())
            {
                return attributes[0];
            }
            return null;
        }

        /// <summary>
        /// Gets all attributes of type T for the specified instance.
        /// </summary>
        /// <typeparam name="T">Attribute type to retrieve.</typeparam>
        /// <param name="instance">The instance decorated with the custom attributes.</param>
        /// <returns>The attributes assigned to the instance.</returns>
        public static T[] GetCustomAttributes<T>(this object instance) where T : Attribute
        {
            return GetCustomAttributes<T>(instance.GetType());
        }

        /// <summary>
        /// Gets all attributes of type T for the specified type.
        /// </summary>
        /// <typeparam name="T">Attribute type to retrieve.</typeparam>
        /// <param name="type">The type decorated with the custom attributes.</param>
        /// <returns>The attributes assigned to the type.</returns>
        public static T[] GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), true);
            if (attributes != null && attributes.Any())
            {
                return attributes.OfType<T>().ToArray();
            }
            return null;
        }

        #endregion

        #region Property
        /// <summary>
        /// The the PropertyInfo for the provided instance with the specified name.
        /// </summary>
        /// <param name="instance">The instance to get the PropertyInfo for.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The PropertyInfo in the instance with the specified name.</returns>
        public static PropertyInfo GetPropertyInfo(this object instance, string propertyName)
        {
            PropertyInfo[] properties = instance.GetType().GetProperties();
            return properties.SingleOrDefault(match => match.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets property value for the specified instance.
        /// </summary>
        /// <typeparam name="T">Property Type.</typeparam>
        /// <param name="propertyInfo">Reflection Property.</param>
        /// <param name="instance">Instance.</param>
        /// <returns>Property value for the instance.</returns>
        public static T GetValue<T>(this PropertyInfo propertyInfo, object instance)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            if (instance == null) throw new ArgumentNullException("instance");
            return (T)propertyInfo.GetValue(instance, null);
        }

        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <typeparam name="T">Property Value Type.</typeparam>
        /// <param name="propertyInfo">Reflection Property.</param>
        /// <param name="instance">Instance.</param>
        /// <param name="value">Value.</param>
        public static void SetValue<T>(this PropertyInfo propertyInfo, object instance, T value)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            if (instance == null) throw new ArgumentNullException("instance");
            propertyInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Return an instance of ReflectionProperty based on specified name.
        /// </summary>
        /// <param name="instance">The instance from which to retrieve the Property.</param>
        /// <param name="propertyName">The Property name</param>
        /// <returns></returns>
        public static ReflectionProperty Property(this object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName.IsNullOrBlank()) throw new ArgumentNullException("propertyName");
            return new ReflectionProperty(instance, instance.GetPropertyInfo(propertyName));
        }
        #endregion

        #region Method
        /// <summary>
        /// The the PropertyInfo for the provided instance with the specified name.
        /// </summary>
        /// <param name="instance">The instance to get the PropertyInfo for.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The PropertyInfo in the instance with the specified name.</returns>
        public static MethodInfo Method(this object instance, string methodName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (methodName.IsNullOrBlank()) throw new ArgumentNullException("methodName");
            MethodInfo method = instance.GetType().GetMethod(methodName);
            return method;
        }

        public static ParameterInfo[] Parameters(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");
            return method.GetParameters();
        }

        public static MethodInfo MethodInfo(this LambdaExpression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            UnaryExpression body = (UnaryExpression)expression.Body;
            MethodCallExpression methodCallExpression = (MethodCallExpression)body.Operand;
            MethodInfo methodInfo = ((ConstantExpression)methodCallExpression.Object).Value as MethodInfo;
            return methodInfo;
        }

        #endregion

        #region Type
        /// <summary>
        /// Converts an unspecified type value to another type. 
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="outputType">The type the input object is to be converted to.</param>
        /// <param name="result">
        /// If the conversion succeeded, contains converted value; otherwise, null value. 
        /// This parameter is passed un-initialized. 
        /// </param>
        /// <returns>true if value was converted successfully; otherwise, false</returns>         
        public static bool TryChangeType(this object value, Type outputType, out object result)
        {
            result = null;
            if (value == null)
            { return false; }

            try
            {
                Type type = outputType;
                if (outputType.IsGenericType && outputType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    type = Nullable.GetUnderlyingType(outputType);
                }

                result = Convert.ChangeType(value, type);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts an unspecified type value to another type. 
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="outputType">The type the input object is to be converted to.</param>
        /// <param name="result">
        /// If the conversion succeeded, contains converted value; otherwise, null value. 
        /// This parameter is passed un-initialized. 
        /// </param>
        /// <returns>true if value was converted successfully; otherwise, false</returns>         
        public static object ToType(this object value, Type outputType, string format = null)
        {
            if (value == null) return null;

            switch (Type.GetTypeCode(outputType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:                
                case TypeCode.UInt16:                    
                case TypeCode.UInt32:                
                case TypeCode.UInt64:                    
                case TypeCode.Int16:                
                case TypeCode.Int32:                    
                case TypeCode.Int64:                
                case TypeCode.Decimal:
                case TypeCode.Double:                
                case TypeCode.Single:
                    object converted = Convert.ChangeType(value, outputType);
                    return !format.IsNullOrBlank() ? converted.Format(format) : converted;
                case TypeCode.DateTime:
                    return !format.IsNullOrBlank() ? DateTime.ParseExact(value.ToString(), format, CultureInfo.InvariantCulture) : DateTime.Parse(value.ToString());
                case TypeCode.String:
                    return value.ToString();
                default:
                    if (outputType.Equals(typeof(Guid)))
                    {
                        return !format.IsNullOrBlank() ? Guid.ParseExact(value.ToString(), format) : Guid.Parse(value.ToString());
                    }
                    else if (outputType.Equals(typeof(OleDateTime)))
                    {
                        return new OleDateTime(Convert.ToDouble(value));
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Conversion from Type '{0}' to '{1}' Is Not Supported.", value.GetType(), outputType));
                    }
            }
        }

        /// <summary>
        /// Converts an unspecified type value to another type. 
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="result">
        /// If the conversion succeeded, contains converted value; otherwise, null value. 
        /// This parameter is passed un-initialized. 
        /// </param>
        /// <returns>true if value was converted successfully; otherwise, false</returns>         
        public static object ToType<T>(this object value, string format = null)
        {
            return value.ToType(typeof(T), format);
        }

        /// <summary>
        /// Determines whether specified Type is numeric (int, uint, decimal, float ... )
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True if specified is numeric; false otherwise.</returns>
        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (type.IsNullableType())
                    { return Nullable.GetUnderlyingType(type).IsNumeric(); }
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether specified Type is Date/Time.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True if specified Type is Date/Time; false otherwise.</returns>
        public static bool IsDateTime(this Type type)
        {
            if (type.Equals(typeof(DateTime)))
            {
                return true;
            }
            else if (type.IsNullableType() && Nullable.GetUnderlyingType(type).IsDateTime())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns fully qualified Type for specified friendly name of type.
        /// </summary>
        /// <param name="typeName">Fully qualified Type or friendly name of type.</param>
        /// <returns>Type.</returns>
        public static Type ToType(this string friendlyTypeName)
        {
            if (friendlyTypeName.IsNullOrBlank()) throw new ArgumentNullException("name");

            Type type;
            if (FriendlyTypeNames.TryGetValue(friendlyTypeName, out type))
            {
                return type;
            }

            type = Type.GetType(friendlyTypeName);
            if (type == null)
            {
                type = Type.GetType(String.Format("System.{0}", friendlyTypeName));
            }

            return type;
        }

        /// <summary>
        /// Provides friendly name for specified Type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Friendly name string representation of Type.</returns>
        public static string FriendlyName(this Type type)
        {
            CodeTypeReference ctr;
            if (type.DerivesFromType(typeof(Nullable<>)))
            {

                ctr = new CodeTypeReference(typeof(Nullable<>));
                ctr.TypeArguments.Add(new CodeTypeReference(System.Nullable.GetUnderlyingType(type)));
            }
            else
            {
                ctr = new CodeTypeReference(type);
            }

            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
            return codeDomProvider.GetTypeOutput(ctr).Replace("System.", String.Empty);
        }



        #endregion

        #region Inheritance
        /// <summary>
        /// Determines if specified Type derives from generic type.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="instanceType">Type.</param>
        /// <returns>True if specified Type derives from generic type; false otherwise.</returns>
        public static bool DerivesFromType<T>(this Type instanceType)
        {
            return instanceType.DerivesFromType(typeof(T));
        }

        /// <summary>
        /// Determines if specified Type derives from type.
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="type">Type.</param>
        /// <returns>True if specified Type derives from type; false otherwise.</returns>
        public static bool DerivesFromType(this Type instanceType, Type type)
        {
            if (instanceType == null) throw new ArgumentNullException("instanceType");
            if (type == null) throw new ArgumentNullException("type");

            Type underlyingType = instanceType.GetImplementedType(type);
            return (underlyingType != null);
        }

        /// <summary>
        /// Searched implementation chain and derived classes for specified generic type.
        /// </summary>
        /// <typeparam name="T">Implementation Type.</typeparam>
        /// <param name="instanceType">Type.</param>
        /// <returns>The spcified Type if implemented.</returns>
        public static Type GetImplementedType<T>(this Type instanceType)
        {
            return instanceType.GetImplementedType(typeof(T));
        }

        /// <summary>
        /// Searched implementation chain and derived classes for specified generic type.
        /// </summary>
        /// <param name="instanceType">Type.</param>
        /// <param name="type">Implementation Type.</param>
        /// <returns>The spcified Type if implemented.</returns>
        public static Type GetImplementedType(this Type instanceType, Type type)
        {
            if (instanceType == null) throw new ArgumentNullException("instanceType");
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsInterface)
            {
                Type[] implementedInterfaces = instanceType.GetInterfaces();
                if (implementedInterfaces != null && implementedInterfaces.Any())
                {
                    return implementedInterfaces.FirstOrDefault(@interface => (@interface.IsGenericType && @interface.GetGenericTypeDefinition().Equals(type))
                        || (@interface.Equals(type)));
                }                
            }
            else
            {
                Type typeDef = instanceType;
                while ((typeDef != null) && !typeDef.Equals(typeof(object)))
                {
                    if ((typeDef.IsGenericType && typeDef.GetGenericTypeDefinition().Equals(type)) || type.Equals(typeDef))
                    { return typeDef; }

                    typeDef = typeDef.BaseType;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether specified Type is Nullable.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True if specified Type is Nullable; false otherwise.</returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Returns the Generic Type for the specified type, ex. IEnumerable<int> returns Int32.
        /// </summary>
        /// <param name="instanceType">Instance Type</param>
        /// <param name="underlyingType"></param>
        /// <returns>The Generic Type.</returns>
        public static Type UnderlyingGenericType(this Type instanceType, Type underlyingType)
        {
            Type genericType = instanceType.GetImplementedType(underlyingType).GetGenericArguments()[0];
            return genericType;
        }

        #endregion

        #region Collection
        public static NameValueCollection ToNameValueCollection(this object instance)
        {
            return DisplayFormatAttribute.ToNameValueCollection(instance);
        }

        #endregion
    }
}
