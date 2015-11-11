using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio
{
    public enum DateGrouping
    {
        [EnumDisplay("Today")]
        Today = 1,

        [EnumDisplay("Tomorrow")]
        Tomorrow = 2,

        [EnumDisplay("This Week")]
        ThisWeek = 3,

        [EnumDisplay("Next Week")]
        NextWeek = 4,
        
        [EnumDisplay("Next Two Weeks")]
        NextTwoWeeks = 5,

        [EnumDisplay("Next Three Weeks")]
        NextThreeWeeks = 6,

        [EnumDisplay("Later This Month")]
        LaterThisMonth = 7,

        [EnumDisplay("Next Month")]
        NextMonth = 8,

        [EnumDisplay("After Next Month")]
        AfterNextMonth = 9,

        [EnumDisplay("Last Week")]
        LastWeek = 10,

        [EnumDisplay("Last Two Weeks")]
        LastTwoWeeks = 11,

        [EnumDisplay("Last Three Weeks")]
        LastThreeWeeks = 12,

        [EnumDisplay("Earlier This Month")]
        EarlierThisMonth = 14,

        [EnumDisplay("Last Month")]
        LastMonth = 14,

        [EnumDisplay("Older")]
        Older = 15,
    }

}
