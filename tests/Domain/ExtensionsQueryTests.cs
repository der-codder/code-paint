using System;
using CodePaint.WebApi.Domain.Models;
using Xunit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace CodePaint.WebApi.Tests.Domain
{
    public class ExtensionsQueryTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        public void NormalizeQueryParams_NormalizingIrrelevantPageNumber_And_PageSize(
            int? pageNumber,
            int? pageSize)
        {
            var query = new ExtensionsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            query.NormalizeQueryParams();

            Assert.Equal(1, query.PageNumber);
            Assert.Equal(50, query.PageSize);
        }

        [Fact]
        public void NormalizeQueryParams_DoesNotChangeValidPageNumber()
        {
            const int expectedPageNumber = 2;
            var query = new ExtensionsQuery
            {
                PageNumber = expectedPageNumber
            };

            query.NormalizeQueryParams();

            Assert.Equal(expectedPageNumber, query.PageNumber);
        }

        [Fact]
        public void NormalizeQueryParams_DoesNotChangeValidPageSize()
        {
            const int expectedPageSize = 20;
            var query = new ExtensionsQuery
            {
                PageSize = expectedPageSize
            };

            query.NormalizeQueryParams();

            Assert.Equal(expectedPageSize, query.PageSize);
        }
    }
}
