using AutoMapper;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Controllers.Resources;
using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to API Resource
            CreateMap<ExtensionMetadata, ExtensionResource>()
                .IncludeBase<ExtensionMetadata, ExtensionMetadataResource>()
                .ForMember(dest => dest.Themes, opt => opt.Ignore());
            CreateMap<ExtensionMetadata, ExtensionMetadataResource>();
            CreateMap<Statistics, StatisticResource>();
            CreateMap<Theme, ThemeResource>()
                // .ForMember(dest => dest.Colors, opt => opt.Ignore())
                .ForMember(dest => dest.TokenColors, opt => opt.Ignore());
            CreateMap<TokenColorSettings, TokenColorSettingsResource>();
            CreateMap<TokenColor, TokenColorResource>();
            CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));

            // API Resource to Domain
            CreateMap<GalleryQueryResource, GalleryQuery>()
                .AfterMap((_, q) => q.NormalizeQueryParams());
        }
    }
}
