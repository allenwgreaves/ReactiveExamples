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
        }
    }
}