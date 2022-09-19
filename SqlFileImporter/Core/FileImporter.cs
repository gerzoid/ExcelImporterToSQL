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
        public abstract string GetNameColumn(int col);
        public abstract string? GetValue(int row, int col);
        
        public abstract IEnumerable<string> GetValues(int row, int col, int rowEnd, int colEnd);
        public abstract int GetCountRows();
        public abstract int GetCountColumns();
        public abstract void CloseFile(string FileName);

    }
}
