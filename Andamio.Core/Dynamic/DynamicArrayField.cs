using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Andamio
{
    public class DynamicArrayField : DynamicField
    {
        #region Constructors
        internal DynamicArrayField(JObject container, string fieldName, int index)
            : base(container, fieldName)
        {
            if (index < 0) throw new IndexOutOfRangeException("index");
            _Index = index;
        }

        protected readonly int _Index;

        #endregion

        #region Optional
        public override DynamicField NotOptional()
        {
            JArray array = _Container[_FieldName] as JArray;
            if (array == null)
            {
                throw new MissingFieldException(System.String.Format("'{0}' Field Not Found Or Not of Array Type.", _FieldName));
            }
            if (array.Count <= _Index)
            {
                throw new IndexOutOfRangeException();
            }
            return this;
        }

        #endregion

        #region String
        public override string String()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Bool
        public override bool Bool()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Nullable
        public override T? Null<T>()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Enum
        public override E Enum<E>(E defaultValue)
        {
            throw new NotImplementedException();
        }

        public override E Enum<E>()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Currency
        public override decimal Currency(decimal defaultValue = 0)
        {
            JArray array = _Container[_FieldName] as JArray;
            if (array != null && array[_Index] != null)
            {
                return array[_Index].ToString().ParseCurrency();
            }

            return defaultValue;
        }

        #endregion

        #region Int
        public override int Int(int defaultValue = 0)
        {
            JArray array = _Container[_FieldName] as JArray;
            if (array != null && array[_Index] != null)
            {
                return array[_Index].Value<int>();
            }

            return defaultValue;
        }

        #endregion

        #region Date
        public override DateTime Date(string format = null)
        {
            return _Container[_FieldName][_Index].ToString().ParseDate();
        }

        #endregion

        #region User
        public override string User()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
