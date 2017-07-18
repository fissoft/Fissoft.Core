using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    [TestClass]
    public class OrderByTest
    {
        private List<OrderByModel> _testModel;

        [TestInitialize]
        public void Init()
        {
            _testModel = new List<OrderByModel>();
            for (var i = 0; i < 10; i++)
            for (var j = 0; j < 10; j++)
            for (var k = 0; k < 10; k++)
                _testModel.Add(new OrderByModel
                {
                    Num1 = i,
                    Num2 = j,
                    Num3 = k,
                    Model = new OrderByModel
                    {
                        Num1 = i,
                        Num2 = j,
                        Num3 = k
                    }
                });
        }

        [TestMethod]
        public void OrderBy()
        {
            var ret = _testModel.AsQueryable().OrderBy("Num2", "asc");
            var take10 = ret.Take(10);
            Assert.AreEqual(10, take10.Count());
            Assert.IsTrue(take10.All(c => c.Num2 == 0));
        }

        [TestMethod]
        public void OrderByDesc()
        {
            var ret = _testModel.AsQueryable().OrderBy("Num2", "desc");
            var take10 = ret.Take(10);
            Assert.AreEqual(10, take10.Count());
            Assert.IsTrue(take10.All(c => c.Num2 == 9));
        }

        [TestMethod]
        public void ThenOrderBy()
        {
            var ret = _testModel.AsQueryable().OrderBy("Num2,Num1", "asc,desc");
            var take10 = ret.Take(10).ToArray();
            Assert.AreEqual(10, take10.Length);
            Assert.IsTrue(take10.All(c => c.Num2 == 0));
            Assert.IsTrue(take10.All(c => c.Num1 == 9));
        }

        [TestMethod]
        public void ThenOrderByDesc()
        {
            var ret = _testModel.AsQueryable().OrderBy("Num2,Num1", "desc");
            var take10 = ret.Take(10).ToArray();
            Assert.AreEqual(10, take10.Length);
            Assert.IsTrue(take10.All(c => c.Num2 == 9));
            Assert.IsTrue(take10.All(c => c.Num1 == 0));
        }

        [TestMethod]
        public void OrderByThenByDesc()
        {
            var ret = _testModel.AsQueryable().OrderBy("Num2,Num1", ",desc");
            var take10 = ret.Take(10).ToArray();
            Assert.AreEqual(10, take10.Length);
            Assert.IsTrue(take10.All(c => c.Num2 == 0));
            Assert.IsTrue(take10.All(c => c.Num1 == 9));
        }

        [TestMethod]
        public void PropThenOrderByDescSU()
        {
            var ret = _testModel.AsQueryable().OrderBy("Model.Num2,Model.Num1", ",desc");
            var take10 = ret.Take(10).ToArray();
            Assert.AreEqual(10, take10.Length);
            Assert.IsTrue(take10.All(c => c.Num2 == 0));
            Assert.IsTrue(take10.All(c => c.Num1 == 9));
        }
    }
}