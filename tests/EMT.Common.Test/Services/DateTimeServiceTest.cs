using EMT.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Test.Services
{
    [TestClass]
    public class DateTimeServiceTest
    {
        #region Fields

        private DateTimeService _service;

        #endregion

        #region

        [TestInitialize]
        public void BeforeTest()
        {
            _service = new DateTimeService();
        }

        [TestCleanup]
        public void AfterTest()
        {
            _service = null;
        }

        #endregion

        #region UnitTimeStart

        [TestMethod]
        public void TestUnixTimeStart()
        {
            DateTime unixTimeStart = _service.UnixTimeStart;
            Assert.AreEqual(DateTimeKind.Unspecified, unixTimeStart.Kind);
            Assert.AreEqual(unixTimeStart.Year, 1970);
            Assert.AreEqual(unixTimeStart.Day, 1);
            Assert.AreEqual(unixTimeStart.Month, 1);
            Assert.AreEqual(unixTimeStart.Hour, 0);
            Assert.AreEqual(unixTimeStart.Minute, 0);
            Assert.AreEqual(unixTimeStart.Second, 0);
            Assert.AreEqual(unixTimeStart.Millisecond, 0);
        }

        #endregion

        #region UnixTimeToDateTime

        [TestMethod]
        public void TestUnixTimeToDateTime_StartTime()
        {
            // 0 -> 1970-01-01T00:00:00.0Z

            long unixTime = 0;

            DateTime actual = _service.UnixTimeToDateTime(unixTime);
            DateTime expected = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(DateTimeKind.Unspecified, actual.Kind);
        }

        [TestMethod]
        public void TestUnixTimeToDateTime()
        {            
            Tuple<long, DateTime>[] samples = new Tuple<long, DateTime>[]
            {
                new Tuple<long, DateTime>(1485402143076, new DateTime(2017, 1, 26, 3, 42, 23, 76, DateTimeKind.Unspecified)),
                new Tuple<long, DateTime>(1485403287666, new DateTime(2017, 1, 26, 4, 1, 27, 666, DateTimeKind.Unspecified))
            };

            foreach(var sample in samples)
            {
                DateTime actual = _service.UnixTimeToDateTime(sample.Item1);
                DateTime expected = sample.Item2;

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(DateTimeKind.Unspecified, actual.Kind);
            }

        }

        #endregion

        #region DateTimeToUnixTime

        [TestMethod]
        public void DateTimeToUnixTime_StartTime_Kind_Unspecified()
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
            long actual = _service.DateTimeToUnixTime(startTime);
            long expected = 0;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateTimeToUnixTime_StartTime_Kind_Utc()
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long actual = _service.DateTimeToUnixTime(startTime);
            long expected = 0;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateTimeToUnixTime_StartTime_Kind_Local()
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            long actual = _service.DateTimeToUnixTime(startTime);
            long expected = 0;
            Assert.AreEqual(expected, actual);
        }


        #endregion
    }
}