using Microsoft.EntityFrameworkCore;

namespace TripService.Api.Data;

public class TripDbContext(DbContextOptions<TripDbContext> options) : DbContext(options), DbContext
{
    public DbSet<Trip> Trips { get; set; } = null!;
}

public class Trip
{
    public int Id { get; set; }
    public string? Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
