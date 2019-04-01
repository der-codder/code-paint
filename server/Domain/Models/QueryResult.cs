using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Domain.Models
{
    public class QueryResult<T>
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
