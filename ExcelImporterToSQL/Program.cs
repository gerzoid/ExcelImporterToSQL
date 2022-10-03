using SqlFileImporter;
using SqlFileImporter.Classes;

namespace ExcelImporterToSQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*SqlFileImport importer = new SqlFileImport(@"c:\dev\pso.xlsx", SqlFileImporter.Enum.IMPORT_TO.SQLServer);
            importer.settings.NumRowHeader = 0;
            importer.settings.CountRowsForAnalyseType= 99999;
            importer.settings.UseTranslitHeader = true;
            importer.settings.TableName = "pso";
            importer.Go();
            
            var s =importer.CreateStatement;
            s = importer.InsertStatement;

            using (StreamWriter writer = new StreamWriter(@$"c:\dev\{importer.settings.TableName}.sql"))
            {
                importer.ToSQL();
            }*/

            SqlFileImport importer = new SqlFileImport(@"d:\gerz\csv.csv", SqlFileImporter.Enum.IMPORT_TO.SQLServer);
            importer.settings.HeaderInFile = true;
            importer.settings.NumRowHeader = 0;
            importer.settings.CountRowsForAnalyseType = 99999;
            importer.settings.UseTranslitHeader = true;
            importer.settings.TableName = "pso";
            importer.Go();

                        var s =importer.CreateStatement;
                        s = importer.InsertStatement;

                        using (StreamWriter writer = new StreamWriter(@$"c:\dev\{importer.settings.TableName}.sql"))
                        {
                            importer.ToSQL();
                        }
        }

    }
}