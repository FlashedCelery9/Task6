namespace Task6.DTO_s.ParticipantsDto;

public record ParticipantToMeetingDto
{
    public int MeetingId { get; init; }
    public string UserEmail { get; init; } = null!;
}