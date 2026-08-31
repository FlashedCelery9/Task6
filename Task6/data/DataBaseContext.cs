using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task6.Models;

namespace Task6.data;

public class MeetingsDBContext : IdentityDbContext<AppUser>
{
    public MeetingsDBContext(DbContextOptions<MeetingsDBContext> options) : base(options)
    {
        
    }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<MeetingParticipants> MeetingParticipants { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<MeetingAttachment> MeetingAttachments { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);  
        modelBuilder.Entity<UserProfile>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<UserProfile>()
            .Property(u => u.UserId)
            .IsRequired();

        modelBuilder.Entity<MeetingParticipants>()
            .HasOne(mp => mp.UserProfile)
            .WithMany(p => p.MeetingParticipants)
            .HasForeignKey(mp => mp.UserProfileId)
            .HasPrincipalKey(p => p.Id);
        modelBuilder.Entity<MeetingParticipants>()
            .HasKey(mp => new { mp.MeetingId, mp.UserProfileId }); // PRIMARY KEY (MeetingId, ParticipantId)
        
        modelBuilder.Entity<MeetingParticipants>()
            .HasOne(mp => mp.Meeting) //Має звязок один до багатьох зі сторони 1 мітинг має багато мп
            .WithMany(m => m.MeetingParticipants) // 
            .HasForeignKey(mp => mp.MeetingId);

        modelBuilder.Entity<Meeting>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Meetings)
            .HasForeignKey(m => m.RoomId);
        
        modelBuilder.Entity<Meeting>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Meetings)
            .HasForeignKey(m => m.RoomId);
        modelBuilder.Entity<MeetingAttachment>()
            .HasOne(ma => ma.Meeting)
            .WithMany(m => m.MeetingAttachments)
            .HasForeignKey(ma => ma.MeetingId);
        modelBuilder.Entity<Meeting>()
            .Property(m => m.UpdatedAt)
            .HasDefaultValueSql(null);

    }
    
}