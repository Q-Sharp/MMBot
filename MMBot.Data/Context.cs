namespace MMBot.Data;

public class Context : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseLazyLoadingProxies();
            options.UseNpgsql($@"Server=127.0.0.1;Port=5433;Database=MMBotDB;Username=postgres;Password=P0stGresSQL2021");
        }
    }

    public Context()
    {
    }

    public Context(DbContextOptions<Context> options = null) : base(options)
    {
    }

    public DbSet<Member> Member { get; set; }
    public DbSet<MemberRoom> MemberRoom { get; set; }
    public DbSet<Clan> Clan { get; set; }
    public DbSet<Season> Season { get; set; }
    public DbSet<GuildSettings> GuildSettings { get; set; }
    public DbSet<Restart> Restart { get; set; }
    public DbSet<Vacation> Vacation { get; set; }
    public DbSet<Channel> Channel { get; set; }
    public DbSet<MemberGroup> MemberGroup { get; set; }
    public DbSet<MMTimer> Timer { get; set; }
    public DbSet<Strike> Strike { get; set; }
    public DbSet<RaidBoss> RaidBoss { get; set; }
    public DbSet<RaidParticipation> RaidParticipation { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);

    public async Task MigrateAsync() => await Database.MigrateAsync();
}
