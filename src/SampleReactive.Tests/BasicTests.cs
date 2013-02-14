using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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

        [TestMethod]
        public void DeltaDistinctUntilChanged()
        {
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var observable = items.ToObservable();
            var resultItems = new List<int>();
            observable.DistinctUntilChanged(
                new DeltaComparer<int>((x, y) => x - y) {Delta = 2})
                      .Subscribe( resultItems.Add );
            CollectionAssert.AreEqual( new List<int> { 1, 4 }, resultItems );
        }

        [TestMethod]
        public void GetItemsTest()
        {
            var observable = GetSearchItems().ToObservable( Scheduler.Default );
            observable
                 .ObserveOnDispatcher()
                      .Subscribe( searchItem => { /*...*/ } );
            Thread.Sleep(-1);
        }

        [TestMethod]
        public void ThrottleTest()
        {
            Subject<string> keyStrokeSubject = new Subject<string>();
            var keyStrokeObservable = keyStrokeSubject.ObserveOn( Scheduler.Default );
            var observable = keyStrokeObservable.SelectMany( search => GetSearchItems( search ) )
                                                .Subscribe( searchItem => Console.WriteLine( searchItem ) );
            Thread.Sleep(TimeSpan.FromSeconds( 10 ));
            keyStrokeSubject.OnNext( "A" );
            Thread.Sleep(TimeSpan.FromSeconds( 10 ));
            keyStrokeSubject.OnNext( "AB" );
            Thread.Sleep(-1);
        }

        private IEnumerable<string> GetSearchItems(string search)
        {
            while ( true )
            {
                yield return search;
            }
            yield break;
        }

        private IEnumerable<int> GetSearchItems()
        {
            while ( true )
            {
                yield return 1;
            }
            yield break;
        }
    }

    public class DeltaComparer<TItem> : IEqualityComparer<TItem>
    {
        public DeltaComparer(Func<TItem, TItem, double> compare )
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