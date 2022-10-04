using OfficeOpenXml;
using SqlFileImporter.Core;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Drawing;
using SqlFileImporter.Core.Records;
using DbfDataReader;

namespace SqlFileImporter.Classes.Importers
{
    internal class DbfFileImporter : FileImporter
    {
        DbfTable dbf;

        public override void CloseFile(string FileName)
        {
            dbf.Close();
            dbf.Dispose();
        }

        public override int GetCountColumns()
        {
            return dbf.Columns.Count;
        }

        public override int GetCountRows()
        {
            return Convert.ToInt32(dbf.Header.RecordCount);
        }

        public override string? GetValue(int row, int col)
        {
            var dbfRecord = new DbfRecord(dbf);

            //dbf.Stream.Seek()
            //return table.Rows[row][col] is not null ? table.Rows[row][col].ToString() : "";
            return null;
        }

        public override IEnumerable<string> GetValues(int col, int startRow, int countRowForAnalyse)
        {
            return null;
        }

        public CsvFileImporter(string FileName) : base(FileName)
        {

        }
    }
}
