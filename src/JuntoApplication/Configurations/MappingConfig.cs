using AutoMapper;
using JuntoApplication.Dto;
using JuntoApplication.Model;

namespace JuntoApplication.Configurations
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserDto, User>().ReverseMap();
                config.CreateMap<UserDto, Admin>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
