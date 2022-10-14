using SqlFileImporter.Core.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter.Core
{
    public abstract class FileImporter
    {
        protected int numRowData = 0;  //Номер строки начала данных
        protected int numRowHeader = 0;//Номер строки с заголовком
        protected string fileName = "";
        public FileImporter(string FileName)
        {
            this.fileName = FileName;
        }
        public abstract string? GetValue(int row, int col);
        
        public abstract IEnumerable<string> GetValues(int col, int startRow, int countRowForAnalyse);
        public abstract int GetCountRows();
        public abstract int GetCountColumns();
        public abstract void CloseFile(string FileName);

        public virtual Column GetTypeColumn(ColumnStat stat, IEnumerable<string>? values)
        {
            {
                if (stat.max_count_letters > 0)
                    return new Column() { type = "nvarchar", size = stat.max_length, prec = 0 };
                if (stat.max_count_letters == 0)
                {
                    //Числа с дробной частью
                    if (stat.max_count_numerics > 0 && stat.max_count_punctuations == 1)
                    {
                        string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        //Проверили разделитель чисел
                        if (values.Where(d => d.IndexOf(separator) > 0).Count() > 0)
                        {
                            int size = values.Where(d => d.IndexOf(separator) > 0).Select(a => a.Substring(0, a.IndexOf(separator)).Length).Max();
                            int prec = values.Where(d => d.IndexOf(separator) > 0).Select(a => a.Substring(a.IndexOf(separator), a.Length - a.IndexOf(separator) - 1).Length).Max();
                            return new Column() { type = "numeric", size = size, prec = prec };
                        }
                        else
                            return new Column() { type = "nvarchar", size = stat.max_length };
                    }
                    //Числа
                    if (stat.max_count_numerics > 0 && stat.max_count_punctuations == 0)
                    {
                        //Если у числового поля в начале стоят нули, то это всяко строка
                        int cnt = values.Where(d => d.IndexOf('0', 0) == 0).Count();
                        if (cnt > 0)
                            return new Column() { type = "nvarchar", size = stat.max_length };
                        else
                            return new Column() { type = "numeric", size = stat.max_length };
                    }

                    //Даты
                    if (stat.max_count_numerics == 8 && stat.max_count_punctuations == 2)
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
                        return new Column() { type = "date", size = 0, prec = 0 };
                    }
                    //Дата и время
                    //if (stat.max_count_numerics == 14 && stat.max_count_punctuations == 2)
                    if (stat.max_count_numerics >= 13 && stat.max_count_punctuations >= 2)
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

        }
    }
}
