using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Controllers.Resources
{
    public class QueryResultResource<T>
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
