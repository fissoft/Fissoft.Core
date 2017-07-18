using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    /// <summary>
    ///     ���� SearchModelTest �Ĳ����ּ࣬��
    ///     �������� SearchModelTest ��Ԫ����
    /// </summary>
    [TestClass]
    public class SearchModelTest
    {
        /// <summary>
        ///     ��ȡ�����ò��������ģ��������ṩ
        ///     �йص�ǰ�������м��书�ܵ���Ϣ��
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TupleTest()
        {
            var list1 = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "4")
            };
            var list2 = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("5", "6"),
                new Tuple<string, string>("3", "4")
            };
            var list3 = list1.Intersect(list2).ToList();
            Assert.AreEqual(Tuple.Create("3", "4"), list3.FirstOrDefault());
            Assert.AreEqual(1, list3.Count);
        }

        [TestMethod]
        public void GetTypeNameTest()
        {
            var a = new {k = 1, p = 2};
            var type = a.GetType();
            Console.WriteLine(type.Name);
        }
    }
}