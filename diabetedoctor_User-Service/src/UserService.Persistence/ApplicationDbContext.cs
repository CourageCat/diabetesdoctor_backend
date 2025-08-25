namespace UserService.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) 
    : DbContext(options)
{
    // public ApplicationDbContext(DbContextOptions options, IPublisher publisher) : base(options)
    // { }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    // }

    public override int SaveChanges()
    {
        _ = PublishDomainEventsAsync();
        var result = base.SaveChanges();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync();
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
    
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<PatientProfile> PatientProfiles { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<CarePlanMeasurementTemplate> CarePlanMeasurements { get; set; }
    public DbSet<CarePlanMeasurementInstance> CarePlanMeasurementInstances { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<DoctorProfile> DoctorProfiles { get; set; }
    public DbSet<HospitalProfile> HospitalProfiles { get; set; }
    public DbSet<HospitalStaff> HospitalStaffs { get; set; }
    public DbSet<HospitalAdmin> HospitalAdmins { get; set; }
    public DbSet<AdminProfile> AdminProfiles { get; set; }
    public DbSet<ModeratorProfile> ModeratorProfiles { get; set; }
    public DbSet<ServicePackage> ServicePackages { get; set; }
    public DbSet<UserPackage> UserPackages { get; set; }
    public DbSet<PaymentHistory> PaymentHistories { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }
    public DbSet<OutboxEventConsumer> OutboxEventConsumers { get; set; }
    
    private async Task PublishDomainEventsAsync()
    {
        var aggregates = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());
        
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}