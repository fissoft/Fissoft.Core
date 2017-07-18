using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    [TestClass]
    public class ClassExtesionsTests
    {
        [TestMethod]
        public void GetPropertyTest()
        {
            TestModel model = null;
            Assert.AreEqual(default(string), model.GetProperty(c => c.Name));
            Assert.AreEqual(default(int), model.GetProperty(c => c.Id));
            model = new TestModel
            {
                Name = "a1",
                Id = 2
            };
            Assert.AreEqual(model.Name, model.GetProperty(c => c.Name));
            Assert.AreEqual(model.Id, model.GetProperty(c => c.Id));
        }

        [TestMethod]
        public void GetPropertyWithDictionaryTest()
        {
            Dictionary<int, TestModel> dict= new Dictionary<int, TestModel>();
            Assert.AreEqual(default(TestModel), dict.GetProperty(1));
            Assert.AreEqual(default(int), dict.GetProperty(1,c=>c.Id));
            dict.Add(1, new TestModel
            {
                Name="a1",Id=2
            });
            Assert.AreEqual(dict[1], dict.GetProperty(1));
            Assert.AreEqual(dict[1].Id, dict.GetProperty(1, c => c.Id));
        }

        class TestModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}