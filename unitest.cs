using System;
using Xunit;
using Moq;
using FunctionApp20;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Internal;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Mock<HttpRequest> http = new Mock<HttpRequest>();
            Mock<ILogger> logger = new Mock<ILogger>();

            var mydictionary = new Dictionary<string, StringValues>();

            http.Setup(aa => aa.Query).Returns(new QueryCollection(mydictionary));

            mydictionary.Add("name", "ritesh");

            var result = FunctionApp20.Function1.Run(http.Object, logger.Object);
            var actualresult = result.Result;
            Assert.Equal("Hello, ritesh", actualresult);

        }
    }
}
