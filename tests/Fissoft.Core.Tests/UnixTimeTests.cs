using Fissoft.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fissoft.Core.Tests
{
    [TestClass]
    public class UnixTimeTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var dateTimeExpect = DateTime.Now;
            var unixtime = UnixTime.FromDateTime(dateTimeExpect);
            var dateTimeActual = UnixTime.FromUnixTime(unixtime);
            
            Assert.IsTrue(Math.Abs((dateTimeExpect - dateTimeActual).TotalSeconds) < 1);
            
        }
        [TestMethod]
        public void ConvertGMT8Test()
        {
            var dateTimeExpect = DateTime.Now;
            var unixtime = UnixTime.FromDateTimeByGMT8(dateTimeExpect);
            var dateTimeActual = UnixTime.FromUnixTime(unixtime);

            Assert.IsTrue(
                (Math.Abs((dateTimeExpect - dateTimeActual).TotalSeconds) - 8 * 60 * 60) < 1);

        }
    }
}
