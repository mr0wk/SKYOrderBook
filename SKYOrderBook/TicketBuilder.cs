using System.Net.Sockets;
using System.Reflection;
using Action = SKYOrderBook.Enum.Action;

namespace SKYOrderBook
{
    public static class TicketBuilder
    {
        private static List<CsvRecord> _ticket = new List<CsvRecord>();
        private static ushort _b0 = 0;
        private static ushort _bq0 = 0;
        private static ushort _bn0 = 0;
        private static ushort _a0 = 0;
        private static ushort _aq0 = 0;
        private static ushort _an0 = 0;
        public static IEnumerable<CsvRecord> Build(IEnumerable<CsvRecord> orders)
        {
            foreach (var order in orders)
            {
                if (order.Action == Action.Y || order.Action == Action.F)
                {
                    ClearTicket();
                    _ticket.Add(order);
                }
                else if (order.Action == Action.A || order.Action == Action.M)
                {
                    var orderExists = CheckOrderExists(order.OrderId);

                    if (orderExists)
                    {
                        UpdateOrder(order);
                        continue;
                    }

                    AddOrder(order);
                }
                else if (order.Action == Action.D)
                {
                    var orderExists = CheckOrderExists(order.OrderId);

                    if (orderExists)
                    {
                        DeleteOrder(order);
                    }
                }

            }

            return _ticket;
        }

        private static void ClearTicket()
        {
            _b0 = _bq0 = _bn0 = _a0 = _aq0 = _an0 = 0;
        }

        private static bool CheckOrderExists(ulong orderId)
        {
            return _ticket.Where(o => o.OrderId.Equals(orderId)).Any();
        }

        private static void UpdateOrder(CsvRecord order)
        {
            var modifiedProperties = new Dictionary<string, object>
            {
                { "Price", order.Price },
                { "Quantity", order.Quantity },
                { "SourceTime", order.SourceTime },
                { "Side", order.Side }
            };

            ModifyOrderProperties(order, modifiedProperties);
            UpdateTicketComponents();

            modifiedProperties = new Dictionary<string, object>
            {
                { "B0", _bq0 },
                { "BQ0", _bq0 },
                { "BN0", _bn0 },
                { "A0", _a0 },
                { "AQ0", _aq0 },
                { "AN0", _an0 }
            };

            ModifyOrderProperties(order, modifiedProperties);
        }

        private static void AddOrder(CsvRecord order)
        {
            _ticket.Add(order);
            UpdateTicketComponents();

            var modifiedProperties = new Dictionary<string, object>
            {
                { "B0", _b0 },
                { "BQ0", _bq0 },
                { "BN0", _bn0 },
                { "A0", _a0 },
                { "AQ0", _aq0 },
                { "AN0", _an0 }
            };

            ModifyOrderProperties(order, modifiedProperties);
        }

        private static void ModifyOrderProperties(CsvRecord order, Dictionary<string, object> modifiedProperties)
        {

            foreach (var kvp in modifiedProperties)
            {
                PropertyInfo property = order.GetType().GetProperty(kvp.Key);

                if (property != null)
                {
                    property.SetValue(order, kvp.Value);
                }
            }
        }

        private static void UpdateTicketComponents()
        {
            if (_ticket.Any(o => o.Side.Equals(1)))
            {
                _b0 = _ticket.Where(o => o.Side.Equals(1)).Max(o => o.Price);
                _bq0 = (ushort)_ticket.Where(o => o.Price.Equals(_b0) && o.Side.Equals(1)).Sum(o => o.Quantity);
                _bn0 = (ushort)_ticket.Count(o => o.Price.Equals(_b0) && o.Side.Equals(1));
            }

            if (_ticket.Any(o => o.Side.Equals(2)))
            {
                _a0 = _ticket.Where(o => o.Side.Equals(2)).Min(o => o.Price);
                _aq0 = (ushort)_ticket.Where(o => o.Price.Equals(_a0) && o.Side.Equals(2)).Sum(o => o.Quantity);
                _an0 = (ushort)_ticket.Count(o => o.Price.Equals(_a0) && o.Side.Equals(2));
            }
        }

        private static void DeleteOrder(CsvRecord order)
        {
            _ticket.Remove(order);
            UpdateTicketComponents();
        }
    }
}
