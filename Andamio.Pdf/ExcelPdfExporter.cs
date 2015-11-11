using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Andamio.Pdf
{
    public sealed class ExcelPdfExportSettings
    {
        public string WorkbookPath { get; set; }
        public string OutputPdfPath { get; set; }
        public string WorksheetName { get; set; }
        public string WorksheetRange { get; set; }
    }


    public static class ExcelPdfExporter
    {
        public static FileInfo Export(ExcelPdfExportSettings exportSettings)
        {
            if (exportSettings == null) throw new ArgumentNullException("exportSettings");
            if (exportSettings.OutputPdfPath.IsNullOrBlank()) throw new ArgumentException("OutputPdfPath is required.");
            
            if (File.Exists(exportSettings.OutputPdfPath))
            { 
                File.Delete(exportSettings.OutputPdfPath); 
            }

            Microsoft.Office.Interop.Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application() { ScreenUpdating = false, DisplayAlerts = false }; ;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = null;

            try
            {
                // Create new instance of Excel and open Workbook
                excelWorkbook = excelApplication.Workbooks.Open(exportSettings.WorkbookPath);
                if (excelWorkbook == null)
                {
                    throw new ApplicationException(String.Format("Specified Workbook '{0}' could not be found or may be corrupt.", exportSettings.WorkbookPath));
                }

                // Get Active sheet and Range to export to Pdf
                var sheets = excelWorkbook.Sheets.OfType<Microsoft.Office.Interop.Excel.Worksheet>();
                Microsoft.Office.Interop.Excel.Worksheet worksheet = sheets.FirstOrDefault(s => s.Name.Equals(exportSettings.WorksheetName, StringComparison.OrdinalIgnoreCase));
                if (worksheet == null)
                {
                    throw new ApplicationException(String.Format("Specified Workbook Sheet '{0}' not found.", exportSettings.WorksheetName));
                }

                Microsoft.Office.Interop.Excel.Range range = worksheet.get_Range(exportSettings.WorksheetRange);
                range.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, exportSettings.OutputPdfPath);

                return new FileInfo(exportSettings.OutputPdfPath);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (excelWorkbook != null)
                { excelWorkbook.Close(); }
                excelApplication.Quit();
                excelApplication = null;
                excelWorkbook = null;
            }
        }
    }
}
