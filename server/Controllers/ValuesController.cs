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
        private IGalleryRefreshService _galleryRefreshService;

        public ValuesController(
            IGalleryInfoRepository repository,
            IVSMarketplaceClient marketplaceClient,
            IGalleryRefreshService galleryRefreshService)
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

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value) { }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}
