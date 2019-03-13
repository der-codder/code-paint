using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;

namespace CodePaint.WebApi.Services {

    public interface IVSMarketplaceClient {
        Task<IEnumerable<ThemeInfo>> GetThemesInfoAsync(int pageNumber, int pageSize);
        Task<Stream> GetVsixFileStream(string publisherName, string vsExtensionName, string version);
    }
}
