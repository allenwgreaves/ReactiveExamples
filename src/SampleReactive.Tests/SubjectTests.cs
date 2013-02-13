using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleReactive.Tests
{
    [TestClass]
    public class SubjectTests
    {
        [TestMethod]
        public void IsAnIObservable()
        {
            Subject<string> subject = new Subject<string>();
            Assert.IsInstanceOfType( subject, typeof(IObservable<string>) );
        }

        [TestMethod]
        public void Subscribe_RunsOnTheCurrentThread()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            Subject<string> subject = new Subject<string>();
            subject.Subscribe( x => Assert.AreEqual<int>( currentThreadId, Thread.CurrentThread.ManagedThreadId ));
            subject.OnNext( "Red Dwarf" );
            subject.OnCompleted();
        }

        [TestMethod]
        public void Subscribe_IsDeferred()
        {
            int i = 0;
            Subject<string> subject = new Subject<string>();
            subject.Subscribe( x => Assert.AreEqual<int>(1, i));
            Assert.AreEqual<int>(0, i);
            i += 1;
            subject.OnNext( "Red Dwarf" );
            subject.OnCompleted();
        }

        [TestMethod]
        public void Where_FiltersOnNextCalls()
        {
            Subject<string> stars = new Subject<string>();
            stars.Where(star => star != "Red Dwarf")
                .Subscribe( star => Assert.AreNotEqual<string>("Red Dwarf", star));
            stars.OnNext( "Brown Dwarf" );
            stars.OnNext( "Red Dwarf" );
            stars.OnNext( "White Dwarf" );
            stars.OnNext( "Supergiant" );
            stars.OnCompleted();
        }

        [TestMethod]
        public void Dispose_DetachesSubscription()
        {
            string currentStar = string.Empty;
            Subject<string> stars = new Subject<string>();
            IDisposable subscription = stars.Subscribe( star => currentStar = star);
            stars.OnNext( "Brown Dwarf" );
            subscription.Dispose();
            stars.OnNext( "Red Dwarf" );
            stars.OnCompleted();
            Assert.AreEqual<string>("Brown Dwarf", currentStar);
        }
    }
}