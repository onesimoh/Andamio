using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ms533052(VS.85).aspx
    /// </summary>
    public enum LanguageCodes
    {
        [EnumDisplay("Neutral", SortOrder=1)]
        Neutral = 0,

        [EnumDisplay("English", SortOrder = 2)]
        English = 1,

        [EnumDisplay("Spanish", SortOrder = 3)]
        Spanish = 2,

        [EnumDisplay("Portuguese", SortOrder = 4)]
        Portuguese = 3,

        [EnumDisplay("French", SortOrder = 5)]
        French = 4,        
    }
}
