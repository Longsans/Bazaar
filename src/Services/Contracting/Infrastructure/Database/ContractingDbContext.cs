namespace Bazaar.Contracting.Infrastructure.Database;

public class ContractingDbContext : DbContext
{
    public ContractingDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contract>(contract =>
        {
            contract.HasOne(c => c.Client)
                .WithMany(p => p.Contracts)
                .IsRequired();

            contract.HasOne(c => c.SellingPlan)
                .WithMany()
                .IsRequired();

            contract.Property(c => c.StartDate).HasColumnType("date");
            contract.Property(c => c.EndDate).HasColumnType("date");

            contract.ToTable(c =>
            {
                c.HasCheckConstraint("CK_Contract_StartBeforeEndDate", "[EndDate] IS NULL OR [StartDate] <= [EndDate]");
                c.HasCheckConstraint("CK_Contract_StartDateFromToday", "[StartDate] >= CAST(GETDATE() as date)");
            });
        });

        modelBuilder.Entity<Client>(client =>
        {
            client.HasOne(c => c.SellingPlan)
                .WithMany()
                .IsRequired();

            client.HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientCascade);

            client.HasIndex(c => c.ExternalId)
                .IsUnique();

            client.HasIndex(c => c.EmailAddress)
                .IsUnique();

            client.Property(c => c.ExternalId)
                .HasComputedColumnSql("CONCAT('CLNT-', [Id])", stored: true)
                .HasColumnName("ExternalId");

            client.ToTable(c => c.HasCheckConstraint(
                $"CK_Client_{ClientCompliance.MinimumAge}AndOlder",
                $"DATEDIFF(year, [DateOfBirth], CAST(GETDATE() as date)) >= {ClientCompliance.MinimumAge} AND [DateOfBirth] < GETDATE()"));
        });

        modelBuilder.Entity<SellingPlan>(plan =>
        {
            plan.ToTable(p =>
            {
                p.HasCheckConstraint(
                    "CK_SellingPlan_PerSaleOrMonthlyFeePositive",
                    "[PerSaleFee] > 0 OR [MonthlyFee] > 0");

                p.HasCheckConstraint(
                    "CK_SellingPlan_RegularFeePositive",
                    "[RegularPerSaleFeePercent] > 0");
            });

        });
    }

    public virtual DbSet<Contract> Contracts { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<SellingPlan> SellingPlans { get; set; }
}
