using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Andamio.Data.Entities;

namespace Andamio.Data.Serialization
{
    public static class EntityCollection
    {
        public static readonly Type GenericTypeDefinition = typeof(EntityCollection<,>);

        public static bool IsEntityCollectionType(Type type)
        {
            Type entityCollectionGenericType;
            return DerivesFromEntityCollectionGenericType(type, out entityCollectionGenericType);
        }
        internal static void AssertEntityCollectionType(Type type)
        {
            if (!IsEntityCollectionType(type))
            {
                throw new InvalidOperationException(String.Format("The specified type is not an '{0}' generic type."
                    , GenericTypeDefinition.ToString())); 
            }
        }

        public static Type GetUnderlyingEntityType(Type entityCollectionType)
        {
            AssertEntityCollectionType(entityCollectionType);
            Type entityCollectionGenericType;
            if (DerivesFromEntityCollectionGenericType(entityCollectionType, out entityCollectionGenericType))
            {
                return entityCollectionGenericType.GetGenericArguments()[0];
            }
            else
            {
                return null;
            }
        }

        public static Type GetUnderlyingEntityKeyType(Type entityCollectionType)
        {
            AssertEntityCollectionType(entityCollectionType);
            Type entityCollectionGenericType;
            if (DerivesFromEntityCollectionGenericType(entityCollectionType, out entityCollectionGenericType))
            {
                return entityCollectionGenericType.GetGenericArguments()[1];
            }
            else
            {
                return null;
            }        
        }

        public static bool IsSurrogatedType(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(EntityCollectionSurrogated<,>)));            
        }

        public static Type ToSurrogatedType(Type entityCollectionType)
        {
            AssertEntityCollectionType(entityCollectionType);
            Type [] underlyingTypes = { EntityCollection.GetUnderlyingEntityType(entityCollectionType), EntityCollection.GetUnderlyingEntityKeyType(entityCollectionType) };
            return typeof(EntityCollectionSurrogated<,>).MakeGenericType(underlyingTypes);
        }

        public static object ToSurrogated(object entityCollection)
        {
            return Activator.CreateInstance(ToSurrogatedType(entityCollection.GetType()), entityCollection);
        }

        public static object CreateFromSurrogated(object surrogatedEntityCollection, Type targetType)
        {
            if (!IsSurrogatedType(surrogatedEntityCollection.GetType()))
            {
                throw new InvalidOperationException(String.Format("The specified type is not an '{0}' generic type."
                    , typeof(EntityCollectionSurrogated<,>).ToString())); 
            }

            Type entityType = surrogatedEntityCollection.GetType().GetGenericArguments()[0];
            Type entityKeyType = surrogatedEntityCollection.GetType().GetGenericArguments()[1];
            Type entityCollectionType = GenericTypeDefinition.MakeGenericType(entityType, entityKeyType);
            
            object entityCollection = Activator.CreateInstance(entityCollectionType, surrogatedEntityCollection);
            return Activator.CreateInstance(targetType, entityCollection);
        }

        private static bool DerivesFromEntityCollectionGenericType(Type type, out Type entityCollectionGenericType)
        {
            Type typeDef = type;
            while ((typeDef != null) && !typeDef.Equals(typeof(object)))
            {
                if (typeDef.IsGenericType && typeDef.GetGenericTypeDefinition().Equals(GenericTypeDefinition))
                {
                    entityCollectionGenericType = typeDef;
                    return true; 
                }
                typeDef = typeDef.BaseType;
            }

            entityCollectionGenericType = null;
            return false;
        }
    }

}
