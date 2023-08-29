using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SKYOrderBook
{
    public static class OutputFileWriter
    {
        public static void CreateFileFromTicket(IEnumerable<CsvRecord> ticket, string outputFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                MissingFieldFound = null
            };

            using (var writer = new StreamWriter(outputFilePath))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                var headerFieldNames = new[]
                {
                    "SourceTime", "Side", "Action", "OrderId", "Price", "Qty",
                    "B0", "BQ0", "BN0", "A0", "AQ0", "AN0"
                };

                foreach (var headerFieldName in headerFieldNames)
                {
                    csvWriter.WriteField(headerFieldName);
                }

                csvWriter.NextRecord();

                foreach (var record in ticket.ToList())
                {
                    csvWriter.WriteRecord(record);
                    csvWriter.NextRecord();
                }
            }

            Console.WriteLine("Records written to output CSV file.");
        }
    }
}
