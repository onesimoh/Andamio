using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Pdf
{
    public enum PdfDocumentSize
    {
        None,
        A0,
        A1,
        A2,
        A3,
        A4,
        A4_Landscape,
        A5,
        A6,
        A7,
        A8,
        A9,
        Legal,
        Legal_Landscape,
        Letter,        
        Letter_Landscape,
    }

    public static class PdfDocumentSizeExtensions
    {
        public static bool IsDefined(this PdfDocumentSize size)
        {
            return size != PdfDocumentSize.None;
        }
    }
}
