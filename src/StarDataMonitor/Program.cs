using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("Options: 0");
            Console.Write(": ");
            string mode = Console.ReadLine();
            switch ( mode )
            {
                case "0":
                    Console.WriteLine("Subscribing and Reading Data");
                    Option0();
                    break;
            }
        }

        private static void Option0()
        {
            var pump = new StarDataPump() {Seed = 1};
            var starObservable = GetStarObservable( pump );
            starObservable.ObserveOn( Scheduler.CurrentThread ).Subscribe( star => Console.WriteLine( "{0}\t{1}", star.StarType, star.Mass ) );
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
