using System;
using System.Collections.Generic;
using System.Linq;
using Fissoft.EntitySearch;
using Fissoft.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    /// <summary>
    ///     这是 QueryableExtensionsTest 的测试类，旨在
    ///     包含所有 QueryableExtensionsTest 单元测试
    /// </summary>
    [TestClass]
    public class QueryableExtensionsTest
    {
        private readonly List<MyClass> _table =
            new List<MyClass>
            {
                new MyClass
                {
                    Id = 1,
                    Name = "attach",
                    Name1 = "b",
                    Age = 10,
                    AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 1)),
                    CanNull = 1,
                    Time1 = new DateTime(2010, 9, 1),
                    Time = new DateTime(2010, 9, 1),
                    Class = new MyClass
                    {
                        Id = 11,
                        Age = 110,
                        Class = new MyClass
                        {
                            Id = 111,
                            Age = 1110
                        }
                    },
                    MyEnum = MyEnum.Public,
                    NMyEnum = MyEnum.Public
                },
                new MyClass
                {
                    Id = 2,
                    Name = "blance",
                    Name1 = "b",
                    Age = 20,
                    AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 3)),
                    Time1 = new DateTime(2010, 9, 3),
                    Time = new DateTime(2010, 9, 3),
                    CanNull = 2,
                    Class = new MyClass
                    {
                        Id = 21,
                        Age = 210,
                        Class = new MyClass
                        {
                            Id = 221,
                            Age = 2210
                        }
                    },
                    MyEnum = MyEnum.Public,
                    NMyEnum = MyEnum.Public
                },
                new MyClass
                {
                    Id = 3,
                    Name = "chsword",
                    Name1 = "b",
                    Age = null,
                    AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 4, 10, 10, 10)),
                    Time1 = new DateTime(2010, 9, 4, 10, 10, 10),
                    Time = new DateTime(2010, 9, 4, 10, 10, 10),
                    Class = new MyClass
                    {
                        Id = 31,
                        Age = 310,
                        Class = new MyClass
                        {
                            Id = 331,
                            Age = 3310
                        }
                    }
                }
            };

        /// <summary>
        ///     Where 的测试
        /// </summary>
        [TestMethod]
        public void EqualIntAndString_One()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = "1"};
            var query = _table.AsQueryable();
            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void PropNotExists()
        {
            var item = new SearchItem {Field = "Id1", Method = SearchMethod.Equal, Value = "1"};
            var query = _table.AsQueryable();

            try
            {
                var actual = query.Where(item);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Id1"));
            }
        }

        [TestMethod]
        public void EqualIntAndInt_One()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 1};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void LessThan_One()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.LessThan, Value = "2"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void LessThanOrEqual_Two()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.LessThanOrEqual, Value = "2"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Like_One()
        {
            var item = new SearchItem {Field = "Name", Method = SearchMethod.Like, Value = "*lanc*"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("blance", actual.FirstOrDefault().Name);
        }

        [TestMethod]
        public void NotLike_One()
        {
            var item = new SearchItem {Field = "Name", Method = SearchMethod.NotContains, Value = "lanc"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
            Assert.IsFalse(actual.Select(c => c.Name).Contains("blance"));
        }

        [TestMethod]
        public void EqualNullable_One()
        {
            var item = new SearchItem {Field = "Age", Method = SearchMethod.Equal, Value = "10"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }


        [TestMethod]
        public void EqualInputIsDateTime_One()
        {
            var item = new SearchItem {Field = "Time", Method = SearchMethod.Equal, Value = "2010-09-01"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void EqualInputIsDateTimeNull()
        {
            const int expectResult = 3;
            var item1 = new SearchItem("Time1", SearchMethod.GreaterThan, "2010-08-31");
            var item2 = new SearchItem("Time1", SearchMethod.LessThan, "2010-09-30");
            var query = _table.AsQueryable();
            var actual = query.Where(new SearchModel {Items = new[] {item1, item2}.ToList()});
            Assert.AreEqual(expectResult, actual.Count());
        }


        [TestMethod]
        public void Equal_1_One()
        {
            var item = new SearchItem {Field = "AddTime", Method = SearchMethod.Equal, Value = "-1111111"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void In()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.In, Value = new[] {1, 2, 3}};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(3, actual.Count());
        }

        [TestMethod]
        public void NotIn()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.NotIn, Value = "2,3"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void InParamIsString()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.In, Value = new[] {"1", "2"}};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void InString_Two()
        {
            var item = new SearchItem {Field = "Name", Method = SearchMethod.In, Value = new[] {"attach", "chsword"}};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void InNull_Two()
        {
            var item = new SearchItem {Field = "CanNull", Method = SearchMethod.In, Value = new[] {"1", "2"}};
            var query = _table.AsQueryable();
            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void InNullToIsNull_Two()
        {
            var item = new SearchItem {Field = "CanNull", Method = SearchMethod.In, Value = new int?[] {1, 2}};
            var query = _table.AsQueryable();
            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void IntToIsNull_Two()
        {
            var item = new SearchItem {Field = "CanNull", Method = SearchMethod.In, Value = new[] {1, 2}};
            var query = _table.AsQueryable();
            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void NotEqual_Two()
        {
            var item = new SearchItem {Field = "Id", Method = SearchMethod.NotEqual, Value = 1};
            var query = _table.AsQueryable();
            var actual = query.Where(item);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void OrEqual_Two()
        {
            var item1 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 1, OrGroup = "a"};
            var item2 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 2, OrGroup = "a"};

            var model = new SearchModel {Items = new[] {item1, item2}.ToList()};
            var query = _table.AsQueryable();
            var actual = query.Where(model);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void AndOrMix_One()
        {
            var item1 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 1, OrGroup = "a"};
            var item2 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 2, OrGroup = "a"};
            var item3 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 3};
            var sm = new SearchModel();
            sm.Items.AddRange(new[] {item1, item2, item3});

            var query = _table.AsQueryable();
            var actual = query.Where(sm);
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void TwoGroup_One()
        {
            var item1 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 1, OrGroup = "a"};
            var item2 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 2, OrGroup = "a"};
            var item3 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 2, OrGroup = "b"};
            var item4 = new SearchItem {Field = "Id", Method = SearchMethod.Equal, Value = 3, OrGroup = "b"};
            var sm = new SearchModel();
            sm.Items.AddRange(new[] {item1, item2, item3, item4});

            var query = _table.AsQueryable();
            var actual = query.Where(sm);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void Equal2Level()
        {
            var item = new SearchItem {Field = "Class.Id", Method = SearchMethod.Equal, Value = "11"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void Equal3Level()
        {
            var item = new SearchItem {Field = "Class.Class.Id", Method = SearchMethod.Equal, Value = "111"};
            var query = _table.AsQueryable();

            var actual = query.Where(item);
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void EmptySearch()
        {
            var query = _table.AsQueryable();

            var actual = query.Where(new SearchModel());
            Assert.AreEqual(3, actual.Count());
        }


        [TestMethod]
        public void NullString()
        {
            var item1 = new SearchItem {Field = "NullString", Method = SearchMethod.Contains, Value = 1, OrGroup = "a"};
            var query = _table.AsQueryable();
            var actual = query.Where(item1);
            Assert.AreEqual(0, actual.Count());
        }


        [TestMethod]
        public void EnumTest()
        {
            var item1 = new SearchItem {Field = "MyEnum", Method = SearchMethod.Equal, Value = "Public"};
            var query = _table.AsQueryable();
            var actual = query.Where(item1);
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void EnumAbleNullTest()
        {
            var item1 = new SearchItem {Field = "NMyEnum", Method = SearchMethod.Equal, Value = "Public"};
            var query = _table.AsQueryable();
            var actual = query.Where(item1);
            Assert.AreEqual(2, actual.Count());
        }
    }
}