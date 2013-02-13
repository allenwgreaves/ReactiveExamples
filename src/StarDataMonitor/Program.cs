using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SampleIntegrationPoint;

namespace StarDataMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Options: 0, 1, 2, 3");
            Console.Write(": ");
            string mode = Console.ReadLine();
            switch ( mode )
            {
                case "0":
                    Console.WriteLine("Subscribing and Reading Data");
                    Option0();
                    break;
                case "1":
                    Console.WriteLine("Window and timestamp of data using ToList");
                    Option1();
                    break;
                case "2":
                    Console.WriteLine("Observing two streams of data, one significantly faster than the other.");
                    Option2();
                    break;
                case "3":
                    Console.WriteLine("Grouping data by type and accumulating data.");
                    Option3();
                    break;
            }
        }

        private static void Option3()
        {
            var pump = new StarDataPump() { Seed = 1 };
            var starObservable = GetStarObservable(pump);
            starObservable.GroupBy( star => star.StarType )
                          .Subscribe(
                                  group =>
                                  group.Scan( new Star(),
                                                   ( accum, star ) =>
                                                   new Star { Mass = accum.Mass + star.Mass, StarType = star.StarType } )
                                       .Subscribe(
                                               star =>
                                               Console.WriteLine( "Total Weight Found For: {0} \t {1}",
                                                                  star.StarType,
                                                                  star.Mass ) ) );
            pump.Start();
            starObservable.Wait();
        }

        private static void Option2()
        {
            var slowPump = new StarDataPump() { Seed = 1 };
            var fastPump = new StarDataPump() { Seed = 2, UpperBound = TimeSpan.FromSeconds(0.25) };
            var slowStarObservable = GetStarObservable(slowPump);
            var fastStarObservable = GetStarObservable(fastPump);
            IObservable<Timestamped<Star>> windowedStarObservable =
                    fastStarObservable.Timestamp()
                                      .Window( TimeSpan.FromSeconds( 1 ), 5 )
                                      .SelectMany( x => x.ToList() )
                                      .SelectMany( x => x.ToObservable() );
            var starObservable =
                    slowStarObservable.Timestamp()
                                      .Merge( windowedStarObservable );
            starObservable.ObserveOn(Scheduler.CurrentThread)
                          .Subscribe(
                                  star =>
                                  Console.WriteLine( "{0}: {1}\t{2}",
                                                     star.Timestamp,
                                                     star.Value.StarType,
                                                     star.Value.Mass ) );
            slowPump.Start();
            fastPump.Start();
            starObservable.Wait();
        }

        private static void Option1()
        {
            // Adds a Timestamp to each item and then waits until at least 5 items have been observed or
            // 4 seconds have passed.
            var pump = new StarDataPump() { Seed = 1 };
            var starObservable = GetStarObservable(pump);
            starObservable.Timestamp()
                          .Window( TimeSpan.FromSeconds( 4 ), 5 )
                          .SelectMany( x => x.ToList() )
                          .SelectMany( x => x.ToObservable()  )
                          .ObserveOn( Scheduler.CurrentThread )
                          .Subscribe(
                                  star =>
                                  Console.WriteLine( "{0}: {1}\t{2}",
                                                     star.Timestamp,
                                                     star.Value.StarType,
                                                     star.Value.Mass ) );
            pump.Start();
            starObservable.Wait();
        }

        private static void Option0()
        {
            var pump = new StarDataPump() {Seed = 1};
            var starObservable = GetStarObservable( pump );
            starObservable.ObserveOn( Scheduler.CurrentThread )
                          .Subscribe( star => Console.WriteLine( "{0}\t{1}", star.StarType, star.Mass ) );
            pump.Start();
            starObservable.Wait();
        }

        private static IObservable<Star> GetStarObservable( StarDataPump pump )
        {
            return Observable.FromEventPattern<NewStarEventArgs>(
                    eventHandler => pump.NewStarFound += eventHandler,
                    eventHandler => pump.NewStarFound -= eventHandler )
                             .Select( eventPattern => eventPattern.EventArgs.Star );
        }
    }
}
