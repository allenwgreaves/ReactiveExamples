using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SampleIntegrationPoint;

namespace StockMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new StockChangeViewModel();
            InitializeComponent();
        }

        private void OnLoaded( object sender, RoutedEventArgs e )
        {
        }
    }

    public class StockChange
    {
        public string StockName { get; set; }
        public double Price { get; set; }
        public double PriceChange { get; set; }
        public string PriceChangeSymbol
        {
            get { return PriceChange >= 0 ? "+" : "-"; }
        }
        public string Display
        {
            get { return string.Format( "{0,-10}   {1,-20:C} {2}{3,-10}",
                StockName, Price, PriceChangeSymbol, PriceChange); }
        }
    }

    public class MarketChange
    {
    }

    public class HousingMarket
    {
        private Subject<MarketChange> MarketChangeSubject { get; set; }
        public IObservable<MarketChange> MarketChanges
        {
            get { return MarketChangeSubject; }
        }

        public void BeginMonitoringMarketChanges()
        {
            /*...*/
            MarketChangeSubject.OnNext( new MarketChange( /*...*/ ) );
            /*...*/
        }
    }

    public class DeltaComparer<TItem> : IEqualityComparer<TItem>
    {
        public DeltaComparer(Func<TItem, TItem, double> compare)
        {
            Compare = compare;
        }

        public double Delta { get; set; }

        public Func<TItem, TItem, double> Compare { get; set; }

        public bool Equals(TItem x, TItem y)
        {
            return Math.Abs(Compare(x, y)) <= Delta;
        }

        public int GetHashCode(TItem obj)
        {
            return obj.GetHashCode();
        }
    }

    public class StockChangeViewModel
    {
        public StockChangeViewModel()
        {
            StockTicker = new StockTicker();
            GetPriceChanges(StockTicker)
                    .GroupBy( stock => stock.StockName )
                    .Subscribe( groupedStocks => groupedStocks
                                     .DistinctUntilChanged(new DeltaComparer<PriceChangedEventArgs>((x, y) => x.Price - y.Price) {Delta = 0.4} )
                                     .Scan( new StockChange(),
                                            ( stockChange, stock ) => new StockChange()
                                            {
                                                    StockName = stock.StockName,
                                                    Price = stock.Price,
                                                    PriceChange =
                                                            stock.Price - stockChange.PriceChange
                                            } )
                                    .ObserveOnDispatcher()
                                    .Subscribe( stockChange => StockChanges.Add( stockChange ) ) );
        }

        private IObservable<PriceChangedEventArgs> GetPriceChanges(StockTicker stockTicker)
        {
            return Observable.FromEventPattern<PriceChangedEventArgs>(
                    eventHandler => stockTicker.OnPriceChanged += eventHandler,
                    eventHandler => stockTicker.OnPriceChanged -= eventHandler)
                             .Select(eventPattern => eventPattern.EventArgs);
        }

        private StockTicker StockTicker { get; set; }

        private ObservableCollection<StockChange> _StockChanges = new ObservableCollection<StockChange>();
        public ObservableCollection<StockChange> StockChanges
        {
            get { return _StockChanges; }
            set { _StockChanges = value; }
        }
    }
}
