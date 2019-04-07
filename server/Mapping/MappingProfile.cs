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
                .ForMember(extMeta => extMeta.Themes, opt => opt.Ignore());
            CreateMap<ExtensionMetadata, ExtensionMetadataResource>();
            CreateMap<Statistics, StatisticResource>();
            CreateMap<Theme, ThemeResource>()
                .ForMember(tr => tr.Base, opt => opt.MapFrom(t => t.ThemeType))
                .ForMember(tr => tr.Colors, opt => opt.Ignore())
                .ForMember(tr => tr.Rules, opt => opt.Ignore());
            CreateMap<TokenColor, TokenColorResource>()
                .ForMember(tcr => tcr.Token, opt => opt.MapFrom(tc => tc.Scope))
                .ForMember(tcr => tcr.Foreground, opt => opt.MapFrom(tc => tc.Settings.Foreground))
                .ForMember(tcr => tcr.FontStyle, opt => opt.MapFrom(tc => tc.Settings.FontStyle))
                .AfterMap((_, tcr) =>
                {
                    if (string.IsNullOrWhiteSpace(tcr.Foreground))
                        tcr.Foreground = null;
                });
            CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));

            // API Resource to Domain
            CreateMap<GalleryQueryResource, GalleryQuery>()
                .AfterMap((_, q) => q.NormalizeQueryParams());
        }
    }
}
