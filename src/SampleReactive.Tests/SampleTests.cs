using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
    }
}
