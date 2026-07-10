using AutoMapper;
using Task6.DTO_s.Clients;
using Task6.Models;

namespace Task6.Profiles;

public class MeetingMappingPforile : Profile
{
    public MeetingMappingPforile()
    {
        CreateMap<Meeting, MeetingDetail>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name));
        CreateMap<Meeting, MeetingTitle>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name)); ;
        CreateMap<Meeting, MeetingCreateProfile>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name));
        CreateMap<MeetingCreateProfile, MeetingTitle>();
        CreateMap<MeetingCreateProfile, Meeting>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
        
    }
    
}