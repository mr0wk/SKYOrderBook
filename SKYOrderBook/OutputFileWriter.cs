using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SKYOrderBook
{
    public static class OutputFileWriter
    {
        public static void CreateFileFromTicket(IEnumerable<CsvRecord> ticket)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                MissingFieldFound = null
            };

            using (var writer = new StreamWriter(@"D:\Dev\SKYOrderBook\SKYOrderBook\Output.csv"))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.WriteRecord("SourceTime;Side;Action;OrderId;Price;Qty;B0;BQ0;BN0;A0;AQ0;AN0");
                csvWriter.NextRecord();

                foreach (var record in ticket)
                {
                    csvWriter.WriteRecord(record);
                    csvWriter.NextRecord();
                }
            }

            Console.WriteLine("Records written to output CSV file.");
        }
    }
}
