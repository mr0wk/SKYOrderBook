using Action = SKYOrderBook.Enum.Action;

namespace SKYOrderBook
{
    public class TicketBuilder
    {
        private Dictionary<ulong, CsvRecord> _processedTicksBid = new Dictionary<ulong, CsvRecord>();
        private Dictionary<ulong, CsvRecord> _processedTicksAsk = new Dictionary<ulong, CsvRecord>();
        private List<CsvRecord> _ticket = new List<CsvRecord>();
        private ushort? _b0 = null;
        private ushort? _bq0 = null;
        private ushort? _bn0 = null;
        private ushort? _a0 = null;
        private ushort? _aq0 = null;
        private ushort? _an0 = null;

        public List<CsvRecord> Build(IEnumerable<CsvRecord> ticks)
        {   
            foreach (var tick in ticks.ToList())
            {
                if (tick.Action == Action.Y || tick.Action == Action.F)
                {
                    ResetTicketComponents();
                    _ticket.Add(tick);
                    continue;
                }

                var processedTicksToOperateOn = tick.Side == 1 ? _processedTicksBid : _processedTicksAsk;

                if (!processedTicksToOperateOn.ContainsKey(tick.OrderId))
                {
                    if (tick.Action == Action.A)
                    {
                        processedTicksToOperateOn.Add(tick.OrderId, tick);
                    }
                }
                else
                {
                    if (tick.Action == Action.M)
                    {
                        processedTicksToOperateOn.Remove(tick.OrderId);
                        processedTicksToOperateOn.Add(tick.OrderId, tick);
                    }
                    else if (tick.Action == Action.D)
                    {
                        processedTicksToOperateOn.Remove(tick.OrderId);
                    }
                }

                UpdateTicketComponents((byte)tick.Side);

                tick.A0 = _a0;
                tick.AQ0 = _aq0;
                tick.AN0 = _an0;
                tick.B0 = _b0;
                tick.BQ0 = _bq0;
                tick.BN0 = _bn0;

                _ticket.Add(tick);
            }

            return _ticket;
        }

        private void ResetTicketComponents()
        {
            _b0 = _bq0 = _bn0 = _a0 = _aq0 = _an0 = null;
        }

        private void UpdateTicketComponents(byte side)
        {
            if (side == 1)
            {
                _b0 = _processedTicksBid.Values.MaxBy(t => t.Price).Price;
                _bq0 = (ushort)_processedTicksBid.Values.Where(t => t.Price.Equals(_b0)).Sum(t => t.Quantity);
                _bn0 = (ushort)_processedTicksBid.Values.Count(t => t.Price == _b0);
            }
            else if (side == 2)
            {
                _a0 = _processedTicksAsk.Values.MinBy(t => t.Price).Price;
                _aq0 = (ushort)_processedTicksAsk.Values.Where(t => t.Price.Equals(_a0)).Sum(t => t.Quantity);
                _an0 = (ushort)_processedTicksAsk.Values.Count(t => t.Price == _a0);
            }
        }
    }
}
