namespace SqlFileImporter.Core.Records
{
    public record ColumnStat
    {
        public int indexColumn;
        public int max_length;
        public int max_count_letters;
        public int max_count_numerics;
        public int max_count_punctuations;
        public int min_count_numerics;
        public int min_count_punctuations;
    }
}
