using AutoMapper;
using WebApi.DTO;
using WebApi.Entities;

namespace WebApi.Helpers
{
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<AppUser, RegisterDto>().ReverseMap();
            }
        }
    
}
