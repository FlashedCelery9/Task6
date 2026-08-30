using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;

namespace Task6.Services.Participant;

public interface IParticipantService
{ 
    Task<MeetingDetail?> AddParticipantToMeetingAsync(ParticipantToMeetingDto meetingDto);
    Task<MeetingDetail?> RemoveParticipantFromMeetingAsync(ParticipantToMeetingDto meetingDto);
    
}