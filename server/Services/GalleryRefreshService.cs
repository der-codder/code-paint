using System;
using CodePaint.WebApi.Models;

namespace CodePaint.WebApi.Services
{
    public class GalleryRefreshService
    {
        private readonly IGalleryRepository _repository;

        public GalleryRefreshService(IGalleryRepository repository)
        {
            _repository = repository;
        }
    }
}
