namespace Bazaar.Contracting.Infrastructure.Repositories;

public class ContractingDbContext : DbContext
{
    public ContractingDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contract>(contract =>
        {
            contract.HasOne(c => c.Partner)
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

        modelBuilder.Entity<Partner>(partner =>
        {
            partner.HasMany(p => p.Contracts)
                .WithOne(c => c.Partner);

            partner.HasIndex(p => p.ExternalId)
                .IsUnique();

            partner.HasIndex(p => p.EmailAddress)
                .IsUnique();

            partner.Property(p => p.ExternalId)
                .HasComputedColumnSql("CONCAT('PNER-', [Id])", stored: true)
                .HasColumnName("ExternalId");

            partner.ToTable(p => p.HasCheckConstraint(
                "CK_Partner_18AndOlder",
                "DATEDIFF(year, [DateOfBirth], CAST(GETDATE() as date)) >= 18 AND [DateOfBirth] < GETDATE()"));
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
    public virtual DbSet<Partner> Partners { get; set; }
    public virtual DbSet<SellingPlan> SellingPlans { get; set; }
}
