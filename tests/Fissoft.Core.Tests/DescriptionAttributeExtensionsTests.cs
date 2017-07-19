using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fissoft.Core.Tests
{
    [TestClass]
    public class DescriptionAttributeExtensionsTests
    {
        [TestMethod]
        public void GetDescription()
        {
            Assert.AreEqual("MyName", TestEnum.Enum1.GetDescription());
            Assert.AreEqual("Enum2", TestEnum.Enum2.GetDescription());
        }

        private enum TestEnum
        {
            [System.ComponentModel.Description("MyName")] Enum1,
            Enum2
        }
    }
}