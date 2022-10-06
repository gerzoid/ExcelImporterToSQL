using SqlFileImporter.Classes;
using SqlFileImporter.Classes.Importers;
using SqlFileImporter.Classes.Utils;
using SqlFileImporter.Core.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter.Core
{
    abstract class Converter
    {
        protected string importFilename;
        protected Settings settings = new Settings();

        protected TranslitMethods.Translitter translitter = new TranslitMethods.Translitter();

        protected string insertStatement = "";
        public string InsertStatement { get => insertStatement; }

        protected string createStatement = "";
        public string CreateStatement { get => createStatement; }

        protected Column[] columns;
        protected FileImporter importer;

        public Converter(string fileName)
        {
            importFilename = fileName;
            if (Path.GetExtension(importFilename) == ".xlsx" || Path.GetExtension(importFilename) == ".xls")
            {
                importer = new ExcelFileImporter(importFilename);
            }
            if (Path.GetExtension(importFilename) == ".csv")
            {
                importer = new CsvFileImporter(importFilename);
            }

        }

        //TODO нужно переделать на плагины
        /*Column GetTypeColumn(ColumnStat stat, IEnumerable<string>? values) { 
        
        }*/

        //TODO почему-то не всегда находится Char.IsPunctuation(d) в строке даты, выдает 0, хотя там две точки
        //Заполнение типа колонок и их длинны исходя из анализа данных в столбцах
        //используемые настройки:
        //settings.UseTranslitHeader
        public void FillColumnsDefinition(int numHeaderRow, int CountRowForAnalyse = 10)
        {
            int countColumns = importer.GetCountColumns();
            int countRows = importer.GetCountRows();

            if (countRows - numHeaderRow <= CountRowForAnalyse)
                CountRowForAnalyse = countRows - settings.NumRowData;

            Console.WriteLine($"Всего колонок {countRows}, столбцов {countColumns}");
            columns = new Column[countColumns];

            int countNoName = 1;

            for (int indexColumn = 0; indexColumn < countColumns; indexColumn++)
            {
                Column tmp = new Column();
                ColumnStat colStat = new ColumnStat();
                //Название столбцов
                if (importer.GetValue(numHeaderRow, indexColumn) == null || importer.GetValue(numHeaderRow, indexColumn) == "")
                {
                    tmp.name = "noname" + countNoName;
                    countNoName++;
                }
                else
                    tmp.name = importer.GetValue(numHeaderRow, indexColumn);

                if (settings.UseTranslitHeader)
                    tmp.name = translitter.Translit(tmp.name, TranslitMethods.TranslitType.Iso).Replace("'", "");  //хак с заменой мягкого знака

                //Проверяем на дубликаты
                int cnt_exists_column = columns.Where(d => d is not null && d.name == tmp.name).Count();
                if (cnt_exists_column > 0)
                    tmp.name = tmp.name + (cnt_exists_column + 1);

                var values = importer.GetValues(indexColumn, settings.NumRowData, CountRowForAnalyse);

                colStat.indexColumn = indexColumn;
                colStat.max_length = values.Max(d => d.Length);
                colStat.max_count_letters = values.Select(d => d.ToCharArray().Where(d => char.IsLetter(d)).Count()).Max(d => d);
                colStat.max_count_numerics = values.Select(d => d.ToCharArray().Where(d => char.IsDigit(d)).Count()).Max(d => d);
                colStat.min_count_numerics = values.Select(d => d.ToCharArray().Where(d => char.IsDigit(d)).Count()).Max(d => d);
                colStat.max_count_punctuations = values.Select(d => d.ToCharArray().Where(d => char.IsPunctuation(d)).Count()).Min(d => d);
                colStat.min_count_punctuations = values.Select(d => d.ToCharArray().Where(d => char.IsPunctuation(d)).Count()).Min(d => d);

                Column tmp2 = importer.GetTypeColumn(colStat, values);
                tmp.type = tmp2.type;
                tmp.size = tmp2.size;
                tmp.prec = tmp2.prec;
                columns[indexColumn] = tmp;
            }
        }
        public abstract void GenerateCreateStatement();
        public abstract void GenerateInsertStatement();
    }
}
