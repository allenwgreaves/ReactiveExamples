using System;

namespace SampleIntegrationPoint
{
    public class PriceChangedEventArgs : EventArgs
    {
        public string StockName { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }
    }

    public class StockTicker
    {
        public event EventHandler<PriceChangedEventArgs> OnPriceChanged;
    }

    public class Star
    {
        public long UniqueID { get; set; }
        public StarType StarType { get; set; }
        public double Mass { get; set; }
    }
}