namespace SqlFileImporter.Core.Records
{
        public record Column
        {
            public string name;
            public string type;
            public int size;
            public int prec = 0;
        }
}

