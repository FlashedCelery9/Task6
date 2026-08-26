using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Migrations;
using Task6.Models;

namespace Task6.Profiles;



public class MeetingMappingPforile : Profile
{
    public MeetingMappingPforile()
    {
        CreateMap<MeetingAttachment, MeetingAttachmentsDto>();
        CreateMap<Meeting, MeetingDetail>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.MeetingParticipants,
                opt => opt.MapFrom(src => src.MeetingParticipants.Select(mp => new ParticipantDto
                    { Name = mp.Participant.Name, Id = mp.Participant.Id })))
            .ForMember(
                dest => dest.MeetingAttachments,
                opt => opt.MapFrom(src => src.MeetingAttachments.Select(ma => new MeetingAttachmentsDto
                        {
                            Id = ma.Id,
                            OriginalName = ma.OriginalName,
                            ContentType = ma.ContentType,
                            UploadedAtUtc = ma.UploadedAtUtc
                        }
                ).ToList())
            );     
        CreateMap<MeetingParticipants, ParticipantDto>();
        CreateMap<Meeting, MeetingCreateProfile>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        CreateMap<MeetingCreateProfile, Meeting>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<Participant, ParticipantDto>();
        CreateMap<Meeting, MeetingReadDto>();

        CreateMap<MeetingCreateDto, Meeting>();



    }
    
}