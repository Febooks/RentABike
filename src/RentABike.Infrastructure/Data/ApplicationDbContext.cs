using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;

namespace RentABike.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");
                }
            }
        }

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.ToTable("Motorcycles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LicensePlate).IsUnique();
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(10);
        });

        modelBuilder.Entity<DeliveryPerson>(entity =>
        {
            entity.ToTable("DeliveryPersons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaxIdNumber).IsUnique();
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TaxIdNumber).IsRequired().HasMaxLength(18);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.ToTable("Rentals");
            entity.HasKey(e => e.Id);
            entity.HasOne<Motorcycle>()
                .WithMany()
                .HasForeignKey(e => e.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<DeliveryPerson>()
                .WithMany()
                .HasForeignKey(e => e.DeliveryPersonId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MotorcycleNotification>(entity =>
        {
            entity.ToTable("MotorcycleNotifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(10);
        });
    }

    public override int SaveChanges()
    {
        ConvertDatesToUtc();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDatesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDatesToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || 
                       e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                var clrType = property.Metadata.ClrType;
                
                if (clrType == typeof(DateTime))
                {
                    var dateTime = (DateTime)property.CurrentValue!;
                    if (dateTime.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = dateTime.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
                            : dateTime.ToUniversalTime();
                    }
                }
                else if (clrType == typeof(DateTime?))
                {
                    var dateTime = (DateTime?)property.CurrentValue;
                    if (dateTime.HasValue && dateTime.Value.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = dateTime.Value.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
                            : dateTime.Value.ToUniversalTime();
                    }
                }
            }
        }
    }
}
