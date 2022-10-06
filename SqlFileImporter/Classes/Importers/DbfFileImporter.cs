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
            
            //dbf.Stream.see
            //return table.Rows[row][col] is not null ? table.Rows[row][col].ToString() : "";
            return null;
        }

        public override IEnumerable<string> GetValues(int col, int startRow, int countRowForAnalyse)
        {
            return null;
        }

        public override Column GetTypeColumn(ColumnStat stat, IEnumerable<string>? values)
        {
            Column column = new Column() { prec = 0, size = 0, type = "nvarchar" };
            
            switch (dbf.Columns[stat.indexColumn].ColumnType)
            {
                case DbfColumnType.Number: column.type = "numeric";
                    column.size = dbf.Columns[stat.indexColumn].ColumnSize ?? 0;
                    column.prec = dbf.Columns[stat.indexColumn].NumericPrecision ?? 0;
                    break;
                case DbfColumnType.Double:
                    column.type = "numeric";
                    column.size = dbf.Columns[stat.indexColumn].ColumnSize ?? 0;
                    column.prec = dbf.Columns[stat.indexColumn].NumericPrecision ?? 0;
                    break;
                case DbfColumnType.SignedLong:
                    column.type = "numeric";
                    column.size = dbf.Columns[stat.indexColumn].ColumnSize ?? 0;
                    column.prec = dbf.Columns[stat.indexColumn].NumericPrecision ?? 0;
                    break;
                case DbfColumnType.Currency: 
                    column.type = "numeric";
                    column.size = dbf.Columns[stat.indexColumn].ColumnSize ?? 0;
                    column.prec = dbf.Columns[stat.indexColumn].NumericPrecision ?? 0;
                    break;
                case DbfColumnType.Float: column.type = "numeric";
                    column.size = dbf.Columns[stat.indexColumn].ColumnSize ?? 0;
                    column.prec = dbf.Columns[stat.indexColumn].NumericPrecision ?? 0;
                    break;
                case DbfColumnType.DateTime:
                    column.type = "datetime";
                    break;
                case DbfColumnType.Date:
                    column.type = "smalldatetime";
                    break;
                default:
                    column.type = "nvarchar";
                    break;

            }
            return column;
        }

        public DbfFileImporter(string FileName) : base(FileName)
        {

        }
    }
}
