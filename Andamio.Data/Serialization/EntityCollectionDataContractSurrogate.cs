using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Andamio.Data.Serialization
{
    public class EntityCollectionDataContractSurrogate : IDataContractSurrogate
    {
        public virtual Type GetDataContractType(Type type)
        {
            // This method is called during serialization and schema export
            if (EntityCollection.IsEntityCollectionType(type))
            {
                return EntityCollection.ToSurrogatedType(type);
            }
            
            return type;
        }

        public virtual object GetObjectToSerialize(object obj, Type targetType)
        {
            if (EntityCollection.IsEntityCollectionType(obj.GetType()) 
                && EntityCollection.IsSurrogatedType(targetType))
            {
                return EntityCollection.ToSurrogated(obj);
            }

            return obj;
        }

        public virtual object GetDeserializedObject(object obj, Type targetType)
        {
            if (EntityCollection.IsSurrogatedType(obj.GetType())
                &&  EntityCollection.IsEntityCollectionType(targetType))
            {
                return EntityCollection.CreateFromSurrogated(obj, targetType);
            }

            return obj;
        }

        public virtual Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }
        public virtual CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            // This method could used to construct an entirely new CLR type when a certain type is imported.
            return typeDeclaration;
        }
        public virtual object GetCustomDataToExport(Type clrType, Type dataContractType)
        {            
            return null;
        }
        public virtual object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {            
            return null;
        }
        public virtual void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {            
        }
    }

}
