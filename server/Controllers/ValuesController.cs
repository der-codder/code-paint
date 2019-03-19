using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using CodePaint.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodePaint.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IGalleryRefreshService _galleryRefreshService;

        public ValuesController(IGalleryRefreshService galleryRefreshService)
        {
            _galleryRefreshService = galleryRefreshService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _galleryRefreshService.RefreshGallery().Wait();

            return new string[] { "value1", "value3", "value2" };
        }
    }
}
