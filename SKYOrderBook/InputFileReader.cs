using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SKYOrderBook
{
    public static class InputFileReader
    {
        public static IEnumerable<CsvRecord>? GetAllCsvRecords(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                MissingFieldFound = null
            };

            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader, config);
            
            return csv.GetRecords<CsvRecord>();
        }
    }
}
