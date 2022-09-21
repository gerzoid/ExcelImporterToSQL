using OfficeOpenXml;
using SqlFileImporter.Core;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Drawing;

namespace SqlFileImporter.Classes
{
    internal class CsvFileImporter : FileImporter
    {
        TextFieldParser parser;
        DataTable table;

        public override void CloseFile(string FileName)
        {
            parser.Close();
            table = null;
            table.Dispose();
        }

        public override int GetCountColumns()
        {
            return table.Columns.Count;
        }

        public override int GetCountRows()
        {
            return table.Rows.Count;
        }

        public override string? GetValue(int row, int col)
        {
            return table.Rows[row][col] is null ? table.Rows[row][col].ToString() : "";
        }

        public override IEnumerable<string> GetValues(int row, int col, int rowEnd, int colEnd)
        {
            List<string> result = new List<string>();
            while (col<=colEnd)
            {
                result.Concat(table.AsEnumerable().Select(s => s.Field<string>(col)).ToList());
                col++;
            }
            return result;
            /*table.AsEnumerable().Where(d=>d[ Select()
            return workSheet.Cells[row+1, col+1, rowEnd+1, colEnd+1].Where(d => d.Value is not null).Select(d => (Convert.ToString(d.Value) ?? ""));*/
        }

        public CsvFileImporter(string FileName) :base(FileName)
        {
            parser = new TextFieldParser(this.fileName);
            string[] colFields;
            colFields = parser.ReadFields();
            while (!parser.EndOfData)
            {
                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    table.Columns.Add(datecolumn);
                }
            }            

        }
    }
}
