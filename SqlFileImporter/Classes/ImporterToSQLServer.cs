using OfficeOpenXml;
using OfficeOpenXml.Drawing.Slicer.Style;
using SqlFileImporter.Core;
using SqlFileImporter.Core.Records;
using System.Text;
using System.Xml.Linq;

namespace SqlFileImporter.Classes
{
    internal class ImporterToSQLServer : ImporterAbstract
    {
        public ImporterToSQLServer(string importFilename) : base(importFilename)
        {
        }

        //Генерация кода создания таблицы
        //используемые настройки:
        //settings.TableName
        public override void GenerateCreateStatement()
        {                        
            StringBuilder sb = new StringBuilder($"create table {settings.TableName}(");
            for (int i = 0; i < columns.Length; i++)
            {
                sb.Append(i > 0 ? "," : "");
                sb.Append ($"[{columns[i].name}] {columns[i].type}");
                if (columns[i].size != 0)
                {
                    sb.Append($"({columns[i].size}");
                    if (columns[i].prec != 0)
                        sb.Append($",{columns[i].prec})");
                    else
                        sb.Append($")");
                }
                sb.Append(" Null\r\n");
            }
            sb.Append(");");
            createStatement = sb.ToString();
        }
        
        //Генерация кода вставки данных в таблицу
        //используемые настройки:
        //settings.TableName
        //settings.NumRowHeader
        //settings.BatchSizeForInsertStatement
        public override void GenerateInsertStatement()
        {
            string insert_tmp = "";

            StringBuilder sb = new StringBuilder($"INSERT INTO {settings.TableName}(");
            StringBuilder sb_result = new StringBuilder("");

            for (int i = 0; i < columns.Length; i++)
            {
                sb.Append(i > 0 ? "," : "");
                sb.Append($"[{columns[i].name}]");
            }
            sb.Append(")\r\n VALUES ");
            insert_tmp = sb.ToString();

            int cnt = 0;
            for (int row = settings.NumRowData; row < importer.GetCountRows(); row++)
            {
                 sb.Append( cnt!= 0 ? ",\r\n" : "").Append("(");
                for (int col = 0; col < columns.Length; col++)
                {
                    sb.Append(col > 0 && col != columns.Length ? ", " : "");
                    string value = importer.GetValue(row, col);
                    value = value.TrimEnd();
                    if (columns[col].type == "Numeric")
                    {
                        if (value != "")
                            sb.Append(value);
                        else
                            sb.Append("NULL");
                    }
                    else
                        sb.Append($"'{value}'");
                }                
                cnt++;
                sb.Append(")");
                if (cnt >= settings.BatchSizeForInsertStatement)
                {
                    sb.Append(";\r\n");
                    sb_result.Append(sb.ToString());
                    sb.Clear();
                    sb.Append(insert_tmp);
                    cnt = 0;
                }
            }
            if (cnt > 0)
                sb_result.Append(sb.ToString()).Append(";\r\n");

            insertStatement = sb_result.ToString();
        }
    }
}

