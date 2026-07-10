using AutoMapper;

namespace Task6.DTO_s.Clients;

public class MeetingCreateProfile : Profile
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartTime { get; set; }
}