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
    
}