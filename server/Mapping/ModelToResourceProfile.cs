using AutoMapper;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Resources;
using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<ExtensionMetadata, ExtensionResource>()
                .ForMember(dest => dest.Statistics, opt => opt.Ignore())
                .ForMember(dest => dest.Themes, opt => opt.Ignore());

            CreateMap<ExtensionStatistic, StatisticResource>();

            CreateMap<Theme, ThemeResource>()
                // .ForMember(dest => dest.Colors, opt => opt.Ignore())
                .ForMember(dest => dest.TokenColors, opt => opt.Ignore());

            CreateMap<TokenColorSettings, TokenColorSettingsResource>();
            CreateMap<TokenColor, TokenColorResource>();
        }
    }
}
