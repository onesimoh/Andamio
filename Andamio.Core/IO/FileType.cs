using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio
{
    public enum FileType
    {
        [EnumDisplay("Generic File")]
        Unknown = 0,

        [EnumDisplay("PDF")]
        Pdf = 1,

        [EnumDisplay("Word")]
        Word = 2,

        [EnumDisplay("Excel")]
        Excel = 3,

        [EnumDisplay("Text")]
        Text = 4,

        [EnumDisplay("Image")]
        Image = 5,

        [EnumDisplay("Html")]
        Html = 6,

        [EnumDisplay("Xml")]
        Xml = 7,

        [EnumDisplay("Binary")]
        Binary = 8
    }
}
