using Task6.DTO_s.ParticipantsDto;

namespace Task6.Services;

public interface IParticipantService
{
    Task<ParticipantCreateDto> CreatePaticipant(ParticipantCreateDto participant);
}