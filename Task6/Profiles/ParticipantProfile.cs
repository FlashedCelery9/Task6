using AutoMapper;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Profiles;

public class ParticipantProfile : Profile
{
    public ParticipantProfile()
    {
        CreateMap<Participant, ParticipantDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));
        CreateMap<ParticipantDto, Participant>();

        CreateMap<Participant, ParticipantCreateDto>()
            .ForMember(d => d.MeetingsId, opt => opt
                .MapFrom(s => s
                    .MeetingParticipants.Select(mp => mp.MeetingId).ToList()));
        
        
    }
}