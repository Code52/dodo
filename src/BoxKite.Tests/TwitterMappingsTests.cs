using System;
using System.Linq;
using BoxKite.Extensions;
using BoxKite.Mappings;
using BoxKite.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BoxKite.Tests
{
    [TestClass]
    public class TwitterMappingsTests : BaseContext
    {
        readonly Func<Tweet, bool> _isDateTimeSet = t => t.Time != DateTimeOffset.MinValue;

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

            Assert.IsTrue(contents.FromResponse().Any());
        }

        [TestMethod]
        public async void FromTweet_UsingSampleData_PopulatesDates()
        {
            var contents = await GetTestData(@"data\timeline.txt");

            var results = contents.FromResponse();

            Assert.IsTrue(results.All(_isDateTimeSet));
        }

        [TestMethod]
        public async void Deserialize_SingleTweet_PopulatesFields()
        {
            var contents = await GetTestData(@"data\sampletweet.txt");

            var tweet = contents.FromTweet();

            Assert.IsTrue(tweet != null);
            Assert.IsTrue(tweet.User != null);
            Assert.IsTrue(tweet.Time != DateTimeOffset.MinValue);
        }

        [TestMethod]
        public async void Deserialize_Retweet_PopulatesFields()
        {
            // @hhariri retweets @wilderminds
            var contents = await GetTestData(@"data\retweet.txt");

            var tweet = contents.FromTweet();

            Assert.IsTrue(tweet != null);
            Assert.IsTrue(tweet.User.Name == "WilderMinds");
            Assert.IsTrue(tweet.RetweetedBy.Name == "hhariri");
        }

        [TestMethod]
        public void ParseDateTime_UsingValidTwitterTime_ReturnsResult()
        {
            var dateTime = "Sun Apr 15 02:31:50 +0000 2012".ToDateTimeOffset();

            Assert.AreNotEqual(DateTimeOffset.MinValue, dateTime);
        }

        [TestMethod]
        public void ParseDateTime_UsingValidTime_ReturnsResult()
        {
            var dateTime = "Sun, 15 Apr 2012 02:31:50 +0000".ToDateTimeOffset();

            Assert.AreNotEqual(DateTimeOffset.MinValue, dateTime);
        }
    }
}
