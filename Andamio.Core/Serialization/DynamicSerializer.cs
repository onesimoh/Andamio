using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;

namespace Andamio.Serialization
{
    public interface IDynamicMarshaling
    {
        dynamic Write();
    }

    public interface IDynamicUnmarshaling
    {
        void Read(dynamic value);
    }

    public static class DynamicSerializer
    {
        /// <summary>
        /// Serializes an object, or graph of objects with the given root to an ExpandoObject dynamic Type.
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to serialize.</typeparam>
        /// <param name="object">The Object to Serialize to a dynamic Type.</param>
        /// <param name="bindingFlags">System.Reflection.BindingFlags that specify what properties to retrieve.</param>        
        public static dynamic Serialize<EntityType>(EntityType @object             
            , BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public
            /*, params KeyValuePair<Expression<Func<EntityType, object>>, Func<EntityType, object>>[] converters*/)
        {
            IDictionary<string, object> serialized = new ExpandoObject();
            if (@object == null) return null;

            PropertyInfo[] properties = typeof(EntityType).GetProperties(bindingFlags);
            if (properties != null && properties.Any())
            {
                foreach (PropertyInfo property in properties)
                {
                    var propertyValue = property.GetValue(@object, null);
                    if (propertyValue == null)
                    {
                        serialized.Add(property.Name, null);
                        continue;
                    }

                    if (propertyValue is Enum)
                    {
                        serialized.Add(property.Name, new { Value = ((Enum) propertyValue).ToString(), Text = ((Enum) propertyValue).DisplayName() });
                    }
                    else if (propertyValue is String)
                    {
                        serialized.Add(property.Name, propertyValue);
                    }                    
                    else if (propertyValue is IDynamicMarshaling)
                    {
                        serialized.Add(property.Name, ((IDynamicMarshaling) propertyValue).Write());
                    }
                    else if (propertyValue is IEnumerable<IDynamicMarshaling>)
                    {
                        serialized.Add(property.Name, ((IEnumerable<IDynamicMarshaling>) propertyValue).Select(value => value.Write()));
                    }
                    else if (propertyValue is IEnumerable)
                    {
                        serialized.Add(property.Name, ((IEnumerable) propertyValue).Values(value => Serialize(value, bindingFlags))); 
                    }
                    else
                    {
                        serialized[property.Name] = propertyValue;
                    }
                }
            }

            return serialized;
        }
    }

}
