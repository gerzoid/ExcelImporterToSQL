using SqlFileImporter.Classes;
using SqlFileImporter.Core;
using SqlFileImporter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter
{
    public class SqlFileImport
    {
        ImporterAbstract importer;
        
        public Settings settings = new Settings();
        public string CreateStatement { get => importer.CreateStatement; }
        public string InsertStatement { get => importer.InsertStatement; }
        public SqlFileImport(string fileName, IMPORT_TO _TO)
        {
            switch (_TO)
            {
                case IMPORT_TO.SQLServer:
                    importer = new ImporterToSQLServer(fileName);
                    break;
                default: throw new NotImplementedException();
            }
        }

        public void Go()
        {
            Settings settings = new Settings();
            importer.FillColumnsDefinition(settings.NumRowHeader, settings.CountRowsForAnalyseType);
            importer.GenerateCreateStatement();
            importer.GenerateInsertStatement();
        }

        public string ToSQL()
        {
            return importer.CreateStatement + "\r\n" + importer.InsertStatement;
        }
    }
}
