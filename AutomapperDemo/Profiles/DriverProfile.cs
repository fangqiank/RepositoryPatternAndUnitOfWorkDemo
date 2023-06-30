using AutoMapper;
using AutomapperDemo.Models;
using AutomapperDemo.Models.Dtos.Incoming;
using AutomapperDemo.Models.Dtos.Outgoing;

namespace AutomapperDemo.Profiles
{
    public class DriverProfile: Profile
    {
        public DriverProfile()
        {
            CreateMap<CreateDto, Driver>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid()
                ))
                .ForMember(
                    dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.FirstName)
                )
                .ForMember(
                    dest => dest.LastName,
                    opt => opt.MapFrom(src => src.LastName)
                )
                .ForMember(
                    dest => dest.DriverNumber,
                    opt => opt.MapFrom(src => (int)src.DriverNumber)
                )
                .ForMember(
                    dest => dest.WorldChampionship,
                    opt => opt.MapFrom(src => (int)src.WorldChampionship)
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => 1)
                )
                .ForMember(
                    dest => dest.DateAdded,
                    opt => opt.MapFrom(src => DateTime.Now)
                )
                .ForMember(
                    dest => dest.DateUpdated,
                    opt => opt.MapFrom(src => DateTime.Now)
                );

            CreateMap<Driver, DriverDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.LastName} {src.FirstName}")
                )
                .ForMember(
                    dest => dest.DriverNumber,
                    opt => opt.MapFrom(src => src.DriverNumber)
                )
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.WorldChampionship,
                    opt => opt.MapFrom(src => src.WorldChampionship)
                );
        }
    }
}
