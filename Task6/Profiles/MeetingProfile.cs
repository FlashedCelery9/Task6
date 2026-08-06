using AutoMapper;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Profiles;

public class MeetingMappingPforile : Profile
{
    public MeetingMappingPforile()
    {
        CreateMap<Meeting, MeetingDetail>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        CreateMap<MeetingParticipants, ParticipantDto>();
        CreateMap<Meeting, MeetingTitle>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title)); ;
        CreateMap<Meeting, MeetingCreateProfile>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        CreateMap<MeetingCreateProfile, MeetingTitle>();
        CreateMap<MeetingCreateProfile, Meeting>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<Participant, ParticipantDto>();
        CreateMap<MeetingCreateDto, Meeting>();



    }
    
}