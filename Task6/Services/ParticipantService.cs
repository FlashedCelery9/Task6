using AutoMapper;
using Task6.data;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Services;

// public class ParticipantService(MeetingsDBContext context,
//     IMapper mapper) : IParticipantService
// {
//     // public async Task<ParticipantCreateDto> CreatePaticipant(ParticipantCreateDto dto)
//     // {
//     //     var participant = new UserProfile();
//     //     participant.User.UserName = dto.Name; 
//     //     participant.User.Email = dto.Email;
//     //     context.UserProfiles.Add(participant);
//     //     await context.SaveChangesAsync();
//     //
//     //     foreach (var id in dto.MeetingsId)
//     //     {
//     //         context.MeetingParticipants.Add(new MeetingParticipants{MeetingId = id, UserProfile = participant.Id});
//     //     }
//     //     await context.SaveChangesAsync();
//     //     return mapper.Map<ParticipantCreateDto>(participant);
//     // }
// }