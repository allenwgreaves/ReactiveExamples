using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleReactive.Tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void ToObservable()
        {
            IEnumerable<int> items = new List<int> { 121, 242, 383 };
            IObservable<int> observableItems = items.ToObservable();
            Assert.IsInstanceOfType( observableItems, typeof(IObservable<int>) );
        }

        [TestMethod]
        public void Subscribe()
        {
            List<int> items = new List<int> { 121, 242, 383 };
            IObservable<int> observableItems = items.ToObservable();
            List<int> resultItems = new List<int>();
            observableItems.Subscribe( resultItems.Add );
            observableItems.Wait();
            CollectionAssert.AreEqual( items, resultItems );
        }

        [TestMethod]
        public void NewItem()
        {
            var observable = new Subject<string>();
            observable.Subscribe( item => Assert.AreEqual<string>( "foo", item ) );
        }

        [TestMethod]
        public void DistincUntilChanged()
        {
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var observable = items.ToObservable();
            var resultItems = new List<int>();
            observable.DistinctUntilChanged( new TwoOffComparer() )
                      .Subscribe( resultItems.Add );
            CollectionAssert.AreEqual( new List<int> { 1, 4 }, resultItems );
        }
    }

    public class TwoOffComparer : IEqualityComparer<int>
    {
        public bool Equals( int x, int y )
        {
            return Math.Abs( x - y ) <= 2;
        }

        public int GetHashCode( int obj )
        {
            return obj.GetHashCode();
        }
    }
}