using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    [TestClass]
    public class DisplayNameAttributeExtensionsTests
    {
        [TestMethod]
        public void GetDisplayName()
        {
            var propertyInfo = typeof(TestModel).GetTypeInfo().GetProperty("Name");
            Assert.AreEqual("MyName", propertyInfo.GetDisplayName());
        }

        class TestModel
        {
            [DisplayName("MyName")]
            public string Name { get; set; }
        }
    }
}