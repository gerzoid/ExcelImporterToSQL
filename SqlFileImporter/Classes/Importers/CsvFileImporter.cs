using OfficeOpenXml;
using SqlFileImporter.Core;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Drawing;
using SqlFileImporter.Core.Records;

namespace SqlFileImporter.Classes.Importers
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
            return table.Rows[row][col] is not null ? table.Rows[row][col].ToString() : "";
        }

        public override IEnumerable<string> GetValues(int col, int startRow, int countRowForAnalyse)
        {
            return table.AsEnumerable().Select(x => x.Field<string>(col) ?? "").Skip(startRow).Take(countRowForAnalyse).ToList();
        }

        public CsvFileImporter(string FileName) : base(FileName)
        {
            parser = new TextFieldParser(fileName);
            table = new DataTable();
            parser.Delimiters = new string[] { ";" };
            string[] colFields;
            colFields = parser.ReadFields();
            for (int i = 0; i < colFields.Length; i++)
            {
                DataColumn datecolumn = new DataColumn("column" + i);
                datecolumn.AllowDBNull = true;
                table.Columns.Add(datecolumn);
            }
            table.Rows.Add(colFields);
            while (!parser.EndOfData)
            {
                table.Rows.Add(parser.ReadFields());
            }

        }
    }
}
