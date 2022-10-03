using SqlFileImporter.Classes;
using SqlFileImporter.Core.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter.Core
{
    abstract class ImporterAbstract: IImporter
    {
        protected string importFilename;
        protected Settings settings = new Settings();

        protected TranslitMethods.Translitter translitter  = new TranslitMethods.Translitter();

        protected string insertStatement = "";
        public string InsertStatement { get => insertStatement; }
        
        protected string createStatement = "";
        public string CreateStatement { get => createStatement; }

        protected Column[] columns;        
        protected FileImporter importer;
        
        public ImporterAbstract(string fileName)
        {
            this.importFilename = fileName;
            if (System.IO.Path.GetExtension(importFilename) == ".xlsx" || System.IO.Path.GetExtension(importFilename) == ".xls")
            {
                importer = new ExcelFileImporter(importFilename);
            }
            if (System.IO.Path.GetExtension(importFilename) == ".csv")
            {
                importer = new CsvFileImporter(importFilename);
            }

        }

        //TODO нужно переделать на плагины
        Column GetTypeColumn(ColumnStat stat, IEnumerable<string>? values)
        {
            if (stat.max_count_letters > 0)
                return new Column() { type = "nvarchar", size = stat.max_length, prec = 0 };
            if (stat.max_count_letters == 0)
            {
                //Числа с дробной частью
                if ((stat.max_count_numerics > 0) && (stat.max_count_punctuations == 1))
                {
                    string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    //Проверили разделитель чисел
                    if (values.Where(d => d.IndexOf(separator) > 0).Count() > 0)
                    {
                        int size = values.Where(d => d.IndexOf(separator) > 0).Select(a => a.Substring(0, a.IndexOf(separator)).Length).Max();
                        int prec = values.Where(d => d.IndexOf(separator) > 0).Select(a => a.Substring(a.IndexOf(separator), a.Length - a.IndexOf(separator) - 1).Length).Max();
                        return new Column() { type = "Numeric", size = size, prec = prec };
                    }
                    else
                        return new Column() { type = "nvarchar", size = stat.max_length };
                }
                //Числа
                if ((stat.max_count_numerics > 0) && (stat.max_count_punctuations == 0))
                {
                    //Если у числового поля в начале стоят нули, то это всяко строка
                    int cnt = values.Where(d => d.IndexOf('0', 0) == 0).Count();
                    if (cnt > 0)
                        return new Column() { type = "nvarchar", size = stat.max_length };
                    else
                        return new Column() { type = "Numeric", size = stat.max_length };
                }

                //Даты
                if ((stat.max_count_numerics == 8) && (stat.max_count_punctuations == 2))
                {
                    byte cnt = 0;
                    DateTime result;
                    foreach (var item in values)
                    {
                        if (item != "")   //Если это даты исключаем пустую строку, та как null
                        {
                            if (!DateTime.TryParse(item, out result))
                                return new Column() { type = "nvarchar", size = stat.max_length };
                            cnt++;
                        }
                        if (cnt > 10)
                            break;
                    }
                    return new Column() { type = "smalldatetime", size = 0, prec = 0 };
                }
                //Дата и время
                if ((stat.max_count_numerics == 14) && (stat.max_count_punctuations == 2))
                {
                    byte cnt = 0;
                    DateTime result;
                    foreach (var item in values)
                    {
                        if (item != "")   //Если это даты исключаем пустую строку, та как null
                        {
                            if (!DateTime.TryParse(item, out result))
                                return new Column() { type = "nvarchar", size = stat.max_length };
                            cnt++;
                        }
                        if (cnt > 10)
                            break;
                    }
                    return new Column() { type = "datetime", size = 0 };
                }

            }
            return new Column() { type = "nvarchar", size = stat.max_length };
        }

        //TODO почему-то не всегда находится Char.IsPunctuation(d) в строке даты, выдает 0, хотя там две точки
        //Заполнение типа колонок и их длинны исходя из анализа данных в столбцах
        //используемые настройки:
        //settings.UseTranslitHeader
        public void FillColumnsDefinition(int numHeaderRow, int CountRowForAnalyse=10)
        {
            int countColumns = importer.GetCountColumns();
            int countRows = importer.GetCountRows();

            if (countRows - numHeaderRow <= CountRowForAnalyse)
                CountRowForAnalyse = countRows - settings.NumRowData;

            Console.WriteLine($"Всего колонок {countRows}, столбцов {countColumns}");
            columns = new Column[countColumns];

            int countNoName = 1;           

            for (int i = 0; i < countColumns; i++)
            {
                Column tmp = new Column();
                ColumnStat colStat = new ColumnStat();
                //Название столбцов
                if ((importer.GetValue(numHeaderRow, i) == null) || (importer.GetValue(numHeaderRow, i) == ""))
                {
                    tmp.name = "noname" + countNoName;
                    countNoName++;
                }
                else
                    tmp.name = importer.GetValue(numHeaderRow, i);

                if (settings.UseTranslitHeader)
                    tmp.name = translitter.Translit(tmp.name, TranslitMethods.TranslitType.Iso).Replace("'","");  //хак с заменой мягкого знака

                //Проверяем на дубликаты
                int cnt_exists_column = columns.Where(d => d is not null && d.name == tmp.name).Count();
                if (cnt_exists_column > 0)
                    tmp.name = tmp.name + (int)(cnt_exists_column + 1);

                //var value = workSheet.Cells[3, i, 10, i];
                //var f = value.Select(d => (Convert.ToString(d.Value) ?? ""));
                var values = importer.GetValues(i, settings.NumRowData, CountRowForAnalyse);
                //var values = importer.GetValues(i, numHeaderRow+1,  CountRowForAnalyse);

                colStat.max_length = values.Max(d => d.Length);
                colStat.max_count_letters = values.Select(d => d.ToCharArray().Where(d => Char.IsLetter(d)).Count()).Max(d => d);
                colStat.max_count_numerics = values.Select(d => d.ToCharArray().Where(d => Char.IsDigit(d)).Count()).Max(d => d);
                colStat.min_count_numerics = values.Select(d => d.ToCharArray().Where(d => Char.IsDigit(d)).Count()).Max(d => d);
                colStat.max_count_punctuations = values.Select(d => d.ToCharArray().Where(d => Char.IsPunctuation(d)).Count()).Min(d => d);
                colStat.min_count_punctuations = values.Select(d => d.ToCharArray().Where(d => Char.IsPunctuation(d)).Count()).Min(d => d);

                Column tmp2 = GetTypeColumn(colStat, values);
                tmp.type = tmp2.type;
                tmp.size = tmp2.size;
                tmp.prec = tmp2.prec;
                columns[i] = tmp;
            }
        }
        public abstract void GenerateCreateStatement();
        public abstract void GenerateInsertStatement();
    }
}
