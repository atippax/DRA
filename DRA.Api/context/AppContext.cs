using System.Text.Json;
public class AppContext : DbContext
{
    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }

    #region Required
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TruckModel>().Property(e => e.id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<AreaModel>().Property(e => e.id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<AssignmentModel>().Property(e => e.id)
       .ValueGeneratedOnAdd();
        modelBuilder.Entity<TruckModel>()
        .Property(b => b.resources)
        .HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))!)
        .IsRequired();
        modelBuilder.Entity<AssignmentModel>()
        .Property(b => b.resources)
        .HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))!)
        .IsRequired();
        modelBuilder.Entity<AreaModel>()
        .Property(b => b.requiredResources)
        .HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))!)
        .IsRequired();
        modelBuilder.Entity<TruckModel>()
        .Property(b => b.timeToTravel)
        .HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))!)
        .IsRequired();
    }
    #endregion
    public DbSet<TruckModel> trucks { get; set; } = null!;
    public DbSet<AreaModel> areas { get; set; } = null!;
    public DbSet<AssignmentModel> assignments { get; set; } = null!;
}

