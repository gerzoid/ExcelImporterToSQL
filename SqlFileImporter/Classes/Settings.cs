using SqlFileImporter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFileImporter.Classes
{
    public class Settings
    {
        //Номер строки заголовка
        static int numRowHeader = 0;
        //Номер строки с данными
        static int numRowData = 0;        
        //Кол-во строк для анализа типа колонки
        static int countRowsForAnalyseType = 10;
        //Транслитерировать название столбцов
        static bool useTranslitHeader = false;
        //Имя таблицы в которую импортируем
        static string tableName = "tableimport";
        //Кол-во строк для пакетоной вставки (МС СКЛ = 1000)
        static int batchSizeForInsertStatement = 500;

        static bool headerInFile = false;

        public bool HeaderInFile { get => headerInFile; set => headerInFile = value; }
        public int BatchSizeForInsertStatement { get => batchSizeForInsertStatement; set => batchSizeForInsertStatement = value; }
        public string TableName { get => tableName; set => tableName = value; }
        public bool UseTranslitHeader { get => useTranslitHeader; set => useTranslitHeader = value; }
        public int CountRowsForAnalyseType { get => countRowsForAnalyseType; set => countRowsForAnalyseType = value; }
        public int NumRowHeader { get => numRowHeader; set  { numRowHeader = value; numRowData = numRowHeader + 1; } }
        public int NumRowData { get => numRowData; set => numRowData = value; }
    }
}
