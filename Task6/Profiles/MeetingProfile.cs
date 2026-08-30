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
            .ForMember(dest => dest.Title, opt => opt
                .MapFrom(src => src.Title))
            .ForMember(dest => dest.MeetingParticipants,opt => opt
                .MapFrom(src => src.MeetingParticipants.ToList()))
            .ForMember(
                dest => dest.MeetingAttachments,
                opt => opt.
                    MapFrom(src => src.MeetingAttachments))
            .ForMember(dest => dest.MeetingAdmin, opt => opt.MapFrom(m => new MeetingAdminDto(m.Admin.Email, m.Admin.UserName)));     
        

        CreateMap<MeetingAttachment, MeetingAttachmentsDto>()
            .ForMember(dest => dest.MeetingId, opt => opt
                .MapFrom(src => src.Meeting.Id));



    }
    
}