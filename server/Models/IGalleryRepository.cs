using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodePaint.WebApi.Models {

    public interface IGalleryRepository {
        Task<IEnumerable<ThemeInfo>> GetAllThemesInfo();

        Task<ThemeInfo> GetThemeInfo(string id);

        Task Create(ThemeInfo themeInfo);

        Task<bool> Update(ThemeInfo themeInfo);

        Task<bool> Delete(string id);
    }
}
