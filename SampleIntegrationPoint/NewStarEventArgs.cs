using System;

namespace SampleIntegrationPoint
{
    public class NewStarEventArgs : EventArgs
    {
        public Star Star { get; private set; }

        public NewStarEventArgs(Star star)
        {
            Star = star;
        }
    }
}