using System;
using System.Threading.Tasks;
using CodePaint.WebApi.Services;
using FluentScheduler;

namespace CodePaint.WebApi.ScheduledJobs
{
    public class RefreshGalleryRegistry : Registry
    {
        public RefreshGalleryRegistry(IGalleryRefreshService refreshingService)
        {
            Schedule(async () => await refreshingService.RefreshGallery())
                .ToRunNow().AndEvery(60).Minutes();
        }
    }
}
