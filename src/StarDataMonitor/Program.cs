using System;
using System.Collections.Generic;
using System.Linq;
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
            var pump = new StarDataPump(1);
            pump.NewStarFound +=
                    ( sender, eventArgs ) =>
                    Console.WriteLine( "{0}\t{1}", eventArgs.Star.StarType, eventArgs.Star.Mass );
            pump.Start();
            while ( true )
                ;
        }
    }
}
