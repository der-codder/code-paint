using System;

namespace CodePaint.WebApi.Controllers.Resources
{
    public class ExtensionsQueryResource
    {
        public string SortBy { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
