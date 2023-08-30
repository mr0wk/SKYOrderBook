using RecordAction = SKYOrderBook.Enums.RecordAction;

namespace SKYOrderBook
{
    public class TicketBuilder
    {
        private Dictionary<ulong, CsvRecord> _processedBidTicks = new Dictionary<ulong, CsvRecord>();
        private Dictionary<ulong, CsvRecord> _processedAskTicks = new Dictionary<ulong, CsvRecord>();
        private Dictionary<ushort, List<CsvRecord>> _processedBidTicksSortedByPrice = new Dictionary<ushort, List<CsvRecord>>();
        private Dictionary<ushort, List<CsvRecord>> _processedAskTicksSortedByPrice = new Dictionary<ushort, List<CsvRecord>>();
        private List<CsvRecord> _ticket = new List<CsvRecord>();
        private ushort? _b0, _bq0, _bn0, _a0, _aq0, _an0;

        public List<CsvRecord> Build(IEnumerable<CsvRecord> ticks)
        {   
            foreach (var tick in ticks.ToList())
            {
                if (ShouldResetTicket(tick))
                {
                    ResetTicketComponents();
                    _ticket.Add(tick);
                    continue;
                }

                ProcessTick(tick);

                UpdateTicketComponents((byte)tick.Side);

                UpdateTicketProperties(tick);

                _ticket.Add(tick);
            }

            return _ticket;
        }

        private bool ShouldResetTicket(CsvRecord tick)
        {
            return tick.Action == RecordAction.Y || tick.Action == RecordAction.F;
        }

        private void ResetTicketComponents()
        {
            _b0 = _bq0 = _bn0 = _a0 = _aq0 = _an0 = null;
        }

        private void ProcessTick(CsvRecord tick)
        {
            var processedTicks = tick.Side == 1 ? _processedBidTicks : _processedAskTicks;
            var processedSortedTicks = tick.Side == 1 ? _processedBidTicksSortedByPrice : _processedAskTicksSortedByPrice;

            if (!processedTicks.ContainsKey(tick.OrderId))
            {
                if (tick.Action == RecordAction.A)
                {
                    AddTickToProcessed(processedTicks, processedSortedTicks, tick);
                }
            }
            else
            {
                if (tick.Action == RecordAction.M)
                {
                    UpdateTickInProcessed(processedTicks, processedSortedTicks, tick);
                }
                else if (tick.Action == RecordAction.D)
                {
                    RemoveTickFromProcessed(processedTicks, processedSortedTicks, tick);
                }
            }
        }

        private void AddTickToProcessed(Dictionary<ulong, CsvRecord> processedTicks, Dictionary<ushort, List<CsvRecord>> processedSortedTicks, CsvRecord tick)
        {
            processedTicks.Add(tick.OrderId, tick);

            if (processedSortedTicks.ContainsKey(tick.Price))
            {
                processedSortedTicks[tick.Price].Add(tick);
            }
            else
            {
                processedSortedTicks.Add(tick.Price, new List<CsvRecord> { tick });
            }
        }

        private void UpdateTickInProcessed(Dictionary<ulong, CsvRecord> processedTicks, Dictionary<ushort, List<CsvRecord>> processedSortedTicks, CsvRecord tick)
        {
            RemoveTickFromProcessed(processedTicks, processedSortedTicks, tick);

            AddTickToProcessed(processedTicks, processedSortedTicks, tick);
        }

        private void RemoveTickFromProcessed(Dictionary<ulong, CsvRecord> processedTicks, Dictionary<ushort, List<CsvRecord>> processedSortedTicks, CsvRecord tick)
        {
            processedTicks.Remove(tick.OrderId);
            processedSortedTicks[tick.Price].RemoveAll(existingTick => existingTick.OrderId == tick.OrderId);
        }

        private void UpdateTicketComponents(byte side)
        {
            if (side == 1)
            {
                UpdateBidComponents();
            }
            else if (side == 2)
            {
                UpdateAskComponents();
            }
        }

        private void UpdateBidComponents()
        {
            _b0 = _processedBidTicks.Values.Max(t => t.Price);
            _bq0 = (ushort)_processedBidTicksSortedByPrice[(ushort)_b0].Sum(t => t.Quantity);
            _bn0 = (ushort)_processedBidTicksSortedByPrice[(ushort)_b0].Count();
        }

        private void UpdateAskComponents()
        {
            _a0 = _processedAskTicks.Values.Min(t => t.Price);
            _aq0 = (ushort)_processedAskTicksSortedByPrice[(ushort)_a0].Sum(t => t.Quantity);
            _an0 = (ushort)_processedAskTicksSortedByPrice[(ushort)_a0].Count();
        }

        private void UpdateTicketProperties(CsvRecord tick)
        {
            tick.A0 = _a0;
            tick.AQ0 = _aq0;
            tick.AN0 = _an0;
            tick.B0 = _b0;
            tick.BQ0 = _bq0;
            tick.BN0 = _bn0;
        }
    }
}
