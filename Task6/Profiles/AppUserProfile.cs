using AutoMapper;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Profiles;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, MeetingAdminDto>()
            .ForMember(u => u.Username, opt => opt.MapFrom(u => u.UserName))
            .ForMember(u => u.Email, opt => opt.MapFrom(u => u.Email));
        CreateMap<AppUser, ParticipantDto>()
            .ForMember(u => u.Name, opt => opt.MapFrom(u => u.UserName))
            .ForMember(u => u.Id , opt => opt.MapFrom(u => u.Id));
        CreateMap<AppUser, UserProfile>()
            .ForMember(u => u.UserId, opt => opt.MapFrom(u => u.Id));
        
            
        CreateMap<MeetingParticipants, ParticipantDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserProfile.User.Id))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.UserProfile.User.UserName));

    }
}