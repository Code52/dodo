using System;
using System.Linq;
using BoxKite.Mappings;
using BoxKite.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxKite.Tests
{
    [TestClass]
    public class TwitterMappingsTests : BaseContext
    {
        readonly Func<Tweet, bool> _isDateTimeSet = t => t.Time != DateTime.MinValue;

        [TestMethod]
        public async void FromSearchResponse_UsingSampleData_CanBeParsed()
        {
            var contents = await GetTestData(@"data\searchresponse.txt");

            Assert.IsTrue(contents.FromSearchResponse().Any());
        }

        [TestMethod]
        public async void FromSearchResponse_UsingSampleData_PopulatesDates()
        {
            var contents = await GetTestData(@"data\searchresponse.txt");

            var results = contents.FromSearchResponse();

            Assert.IsTrue(results.All(_isDateTimeSet));
        }

        [TestMethod]
        public async void FromTweet_UsingSampleData_CanBeParsed()
        {
            var contents = await GetTestData(@"data\timeline.txt");

            Assert.IsTrue(contents.FromTweet().Any());
        }

        [TestMethod]
        public async void FromTweet_UsingSampleData_PopulatesDates()
        {
            var contents = await GetTestData(@"data\timeline.txt");

            var results = contents.FromTweet();

            Assert.IsTrue(results.All(_isDateTimeSet));
        }
    }
}
