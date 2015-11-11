using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio
{
    public enum Month
    {
        [EnumDisplay("", IsSelectable = false)]
        None = 0,

        [EnumDisplay("January")]
        Jan = 1,
        
        [EnumDisplay("February")]
        Feb = 2,
        
        [EnumDisplay("March")]
        Mar = 3,
        
        [EnumDisplay("April")]
        Apr = 4,
        
        [EnumDisplay("May")]
        May = 5,
        
        [EnumDisplay("June")]
        Jun = 6,
        
        [EnumDisplay("July")]
        Jul = 7,
        
        [EnumDisplay("August")]
        Aug = 8,
        
        [EnumDisplay("September")]
        Sep = 9,

        [EnumDisplay("October")]
        Oct = 10,

        [EnumDisplay("November")]
        Nov = 11,

        [EnumDisplay("December")]
        Dec = 12,
    }

    public static class MonthsExtensions
    {
        public static bool IsDefined(this Month month)
        {
            return month != Month.None;
        }
    }

}
