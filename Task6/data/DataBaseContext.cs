using System.Data;
using Microsoft.EntityFrameworkCore;
using Task6.Models;

namespace Task6.data;

public class MeetingsDBContext : DbContext
{
    public MeetingsDBContext(DbContextOptions<MeetingsDBContext> options) : base(options)
    {
        
    }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<MeetingParticipants> MeetingParticipants { get; set; }
    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetingParticipants>()
            .HasKey(mp => new { mp.MeetingId, mp.ParticipantId }); // PRIMARY KEY (MeetingId, ParticipantId)
        
        modelBuilder.Entity<MeetingParticipants>()
            .HasOne(mp => mp.Meeting) //Має звязок один до багатьох зі сторони 1 мітинг має багато мп
            .WithMany(m => m.MeetingParticipants) // 
            .HasForeignKey(mp => mp.MeetingId);
        
        modelBuilder.Entity<MeetingParticipants>()
            .HasOne(mp => mp.Participant)
            .WithMany(p => p.MeetingParticipants)
            .HasForeignKey(mp => mp.ParticipantId);
        
        modelBuilder.Entity<Meeting>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Meetings)
            .HasForeignKey(m => m.RoomId);
    }
    
}