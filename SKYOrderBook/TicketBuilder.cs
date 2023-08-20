using System.Diagnostics;
using System.Reflection;
using Action = SKYOrderBook.Enum.Action;

namespace SKYOrderBook
{
    public static class TicketBuilder
    {
        private static List<CsvRecord> _ticket = new List<CsvRecord>();
        private static ushort? _b0 = null;
        private static ushort? _bq0 = null;
        private static ushort? _bn0 = null;
        private static ushort? _a0 = null;
        private static ushort? _aq0 = null;
        private static ushort? _an0 = null;

        public static IEnumerable<CsvRecord> Build(IEnumerable<CsvRecord> orderRequests)
        {
            Stopwatch stopwatch = new Stopwatch();
            foreach (var orderRequest in orderRequests)
            {
                stopwatch.Start();
                if (orderRequest.Action == Action.Y || orderRequest.Action == Action.F)
                {
                    ClearTicket();
                    _ticket.Add(orderRequest);
                }
                else if (orderRequest.Action == Action.A || orderRequest.Action == Action.M)
                {
                    var orderExists = CheckOrderExists(orderRequest.OrderId);

                    if (orderExists)
                    {
                        UpdateOrder(orderRequest);
                    }

                    AddOrderRequest(orderRequest);
                }
                else if (orderRequest.Action == Action.D)
                {
                    var orderExists = CheckOrderExists(orderRequest.OrderId);

                    if (orderExists)
                    {
                        DeleteOrder(orderRequest.OrderId);
                        AddOrderRequest(orderRequest);
                    }
                    else
                    {
                        AddOrderRequest(orderRequest);
                    }
                }

                var elapsed = stopwatch.Elapsed;
                Console.WriteLine($"Iteration executed in {elapsed.TotalSeconds:F2}.");
            }

            return _ticket;
        }

        private static void ClearTicket()
        {
            _b0 = _bq0 = _bn0 = _a0 = _aq0 = _an0 = null;
        }

        private static bool CheckOrderExists(ulong orderId)
        {
            return _ticket.Where(o => o.OrderId.Equals(orderId)).Any();
        }

        private static void UpdateOrder(CsvRecord upToDateOrderRequest)
        {
            var outdatedOrderRequests = _ticket.Where(o => o.OrderId.Equals(upToDateOrderRequest.OrderId) && o.SourceTime != upToDateOrderRequest.SourceTime).ToList();

            var modifiedProperties = new Dictionary<string, object>
            {
                { "IsOutdated", true },
            };

            foreach (var outdatedOrderRequest in outdatedOrderRequests)
            {
                ModifyOrderRequestProperties(outdatedOrderRequest, modifiedProperties);
            }
        }

        private static void AddOrderRequest(CsvRecord request)
        {
            _ticket.Add(request);
            UpdateTicket();

            var modifiedProperties = new Dictionary<string, object>
            {
                { "B0", _b0 },
                { "BQ0", _bq0 },
                { "BN0", _bn0 },
                { "A0", _a0 },
                { "AQ0", _aq0 },
                { "AN0", _an0 }
            };

            ModifyOrderRequestProperties(request, modifiedProperties);
        }

        private static void UpdateTicket()
        {
            foreach (var order in _ticket)
            {

            }

            var bidOrdersValidForCalculations = _ticket.Where(o => o.Side.Equals("1") && o.Action != Action.Y && o.Action != Action.F && o.Action != Action.D && !o.IsDeleted && !o.IsOutdated);

            if (bidOrdersValidForCalculations.Any())
            {
                _b0 = bidOrdersValidForCalculations.Max(o => o.Price);
                _bq0 = (ushort)bidOrdersValidForCalculations.Where(o => o.Price.Equals(_b0)).Sum(o => o.Quantity);
                _bn0 = (ushort)bidOrdersValidForCalculations.Count(o => o.Price.Equals(_b0));
            }

            var askOrdersValidForCalculations = _ticket.Where(o => o.Side.Equals("2") && o.Action != Action.Y && o.Action != Action.F && o.Action != Action.D && !o.IsDeleted && !o.IsOutdated);

            if (askOrdersValidForCalculations.Any())
            {
                _a0 = askOrdersValidForCalculations.Min(o => o.Price);
                _aq0 = (ushort)askOrdersValidForCalculations.Where(o => o.Price.Equals(_a0)).Sum(o => o.Quantity);
                _an0 = (ushort)askOrdersValidForCalculations.Count(o => o.Price.Equals(_a0));
            }
        }

        private static void DeleteOrder(ulong orderId)
        {
            var orderRequestsToDelete = _ticket.Where(o => o.OrderId.Equals(orderId)).ToList();

            var modifiedProperties = new Dictionary<string, object>
            {
                { "IsDeleted", true }
            };

            foreach (var orderRequest in orderRequestsToDelete)
            {
                ModifyOrderRequestProperties(orderRequest, modifiedProperties);
            }
        }

        private static void ModifyOrderRequestProperties(CsvRecord orderRequest, Dictionary<string, object> modifiedProperties)
        {

            foreach (var kvp in modifiedProperties)
            {
                PropertyInfo property = orderRequest.GetType().GetProperty(kvp.Key);

                if (property != null)
                {
                    property.SetValue(orderRequest, kvp.Value);
                }
            }
        }
    }
}
