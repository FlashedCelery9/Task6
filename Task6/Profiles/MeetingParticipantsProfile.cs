using AutoMapper;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Profiles;

public class MeetingParticipantsProfile : Profile
{
    public MeetingParticipantsProfile()
    {
        CreateMap<UserProfile, ParticipantDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src=> src.User.UserName))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id));
    }
}