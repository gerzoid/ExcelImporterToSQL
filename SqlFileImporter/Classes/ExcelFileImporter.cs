using OfficeOpenXml;
using OfficeOpenXml.ExternalReferences;
using SqlFileImporter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter.Classes
{
    internal class ExcelFileImporter : FileImporter
    {
        ExcelPackage excel;
        ExcelWorksheet workSheet;

        public override void CloseFile(string FileName)
        {
            excel.Dispose();
        }

        public override int GetCountColumns()
        {
            return workSheet.Dimension.End.Column-1;
        }

        public override int GetCountRows()
        {
            return workSheet.Dimension.End.Row-1;
        }

        public override string? GetValue(int row, int col)
        {
            return workSheet.Cells[row + 1, col+1].Value != null ? Convert.ToString(workSheet.Cells[row + 1, col+1].Value) : "";
        }

        public override IEnumerable<string> GetValues(int col, int  startRow, int countRowForAnalyse)
        {
            int count_rows = countRowForAnalyse>GetCountRows() ? GetCountRows() : countRowForAnalyse;
            return workSheet.Cells[startRow+1, col+1, count_rows, col+1].Where(d => d.Value is not null).Select(d => (Convert.ToString(d.Value) ?? ""));
        }

        public ExcelFileImporter(string FileName) :base(FileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            excel = new ExcelPackage(this.fileName);
            workSheet = excel.Workbook.Worksheets.First();
        }
    }
}
