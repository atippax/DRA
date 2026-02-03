using TruckApi.Models;
public class AppContext : DbContext
{
    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }

    public DbSet<TruckItem> truckItems { get; set; } = null!;
}