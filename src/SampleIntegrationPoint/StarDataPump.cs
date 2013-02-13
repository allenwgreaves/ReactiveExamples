﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleIntegrationPoint
{
    public class StarDataPump
    {
        public StarDataPump()
        {
            Seed = (int) DateTime.Now.Ticks;
        }

        public StarDataPump(int seed)
        {
            Seed = seed;
        }

        private Task CurrentTask { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        protected CancellationToken CancellationToken { get; set; }

        public EventHandler<NewStarEventArgs> NewStarFound = delegate { };

        public int Seed { get; set; }

        public void Start()
        {
            Stop();
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            StarDataPump dataPump = new StarDataPump();
            CurrentTask = Task.Factory.StartNew( dataPump.OnStart );
        }

        private void OnStart()
        {
            Random random = new Random(Seed);
            long uniqueID = 0;
            while ( true )
            {
                if ( CancellationToken.IsCancellationRequested )
                {
                    return;
                }
                double wait = random.NextDouble();
                Thread.Sleep(TimeSpan.FromSeconds( wait ));
                int starTypeIndex = random.Next(Enum.GetValues( typeof (StarType) ).Length);
                Star star = new Star
                {
                        UniqueID = ++uniqueID,
                        StarType = Enum.GetValues( typeof (StarType) ).Cast<StarType>().ElementAt( starTypeIndex ),
                        Mass = random.NextDouble() * 1000000000000
                };
                NewStarFound(this, new NewStarEventArgs( star ));
            }
        }

        public void Stop()
        {
            if ( CurrentTask == null )
            {
                return;
            }
            CancellationTokenSource.Cancel();
            CurrentTask.Wait();
            CurrentTask.Dispose();
            CurrentTask = null;
        }
    }
}
