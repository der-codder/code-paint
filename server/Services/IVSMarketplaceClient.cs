using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;

namespace CodePaint.WebApi.Services {

    public interface IVSMarketplaceClient {
        Task<IEnumerable<ThemeInfo>> GetThemesInfoAsync(int pageNumber, int pageSize);
    }
}
