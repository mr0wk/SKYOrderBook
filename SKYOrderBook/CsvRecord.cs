using CsvHelper.Configuration.Attributes;
using SKYOrderBook.Enum;
using Action = SKYOrderBook.Enum.Action;

namespace SKYOrderBook
{
    public class CsvRecord
    {
        [Name("SourceTime")]
        public string SourceTime { get; set; }

        [Name("Side")]
        public string Side { get; set; }

        [Name("Action")]
        public Action Action { get; set; }

        [Name("OrderId")]
        public ulong OrderId { get; set; }

        [Name("Price")]
        public ushort Price { get; set; }

        [Name("Qty")]
        public byte Quantity { get; set; }

        [Name("B0")]
        public ushort? B0 { get; set; }

        [Name("BQ0")]
        public ushort? BQ0 { get; set; }

        [Name("BN0")]
        public ushort? BN0 { get; set; }

        [Name("A0")]
        public ushort? A0 { get; set; }

        [Name("AQ0")]
        public ushort? AQ0 { get; set; }

        [Name("AN0")]
        public ushort? AN0 { get; set; }
    }
}
