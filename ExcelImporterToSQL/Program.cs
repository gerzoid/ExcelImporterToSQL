using SqlFileImporter;
using SqlFileImporter.Classes;

namespace ExcelImporterToSQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlFileImport importer = new SqlFileImport(@"c:\dev\pso.xlsx", SqlFileImporter.Enum.IMPORT_TO.SQLServer);
            importer.settings.NumRowHeader = 0;
            importer.settings.CountRowsForAnalyseType= 99999;
            importer.settings.UseTranslitHeader = true;
            importer.settings.TableName = "pso";
            importer.Go();
            
            var s =importer.CreateStatement;
            s = importer.InsertStatement;

            using (StreamWriter writer = new StreamWriter(@$"c:\dev\{importer.settings.TableName}.sql"))
            {
                writer.WriteLine(importer.CreateStatement);
                writer.WriteLine("");
                writer.WriteLine(importer.InsertStatement);
            }
        }

    }
}