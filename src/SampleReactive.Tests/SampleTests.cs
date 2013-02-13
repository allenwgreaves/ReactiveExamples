using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleReactive.Tests
{
    [TestClass]
    public class SampleTests
    {
        [TestMethod]
        public void ToObservable()
        {
            IEnumerable<int> items = new List<int> { 121, 242, 383 };
            IObservable<int> observableItems = items.ToObservable();
            Assert.IsInstanceOfType( observableItems, typeof(IObservable<int>) );
        }

        [TestMethod]
        public void ToObservable_Subscribe_RunsOnTheCurrentThread()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            IEnumerable<int> items = new List<int> { 121, 242, 383 };
            items.ToObservable().Subscribe(x => Assert.AreEqual<int>(currentThreadId, Thread.CurrentThread.ManagedThreadId) );
        }

        [TestMethod]
        public void ToObserverableWithSchedulerDefault_Subscribe_RunsOnADifferentThread()
        {
            AutoResetEvent @event = new AutoResetEvent( false );
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            IEnumerable<int> items = new List<int> { 121, 242, 383 };
            items.ToObservable( Scheduler.Default ).Subscribe( x => Assert.AreNotEqual<int>( currentThreadId, Thread.CurrentThread.ManagedThreadId ),
            () => @event.Set());
            @event.WaitOne();
        }
    }
}
