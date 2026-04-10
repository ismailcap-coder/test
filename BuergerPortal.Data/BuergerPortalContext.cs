using Microsoft.EntityFrameworkCore;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Data
{
    public class BuergerPortalContext : DbContext
    {
        public BuergerPortalContext(DbContextOptions<BuergerPortalContext> options)
            : base(options)
        {
        }

        public DbSet<Citizen> Citizens => Set<Citizen>();
        public DbSet<PublicOffice> PublicOffices => Set<PublicOffice>();
        public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
        public DbSet<ServiceApplication> ServiceApplications => Set<ServiceApplication>();
        public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<FeeSchedule> FeeSchedules => Set<FeeSchedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Citizen configuration
            modelBuilder.Entity<Citizen>()
                .HasKey(c => c.CitizenId);
            modelBuilder.Entity<Citizen>()
                .Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Citizen>()
                .Property(c => c.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Citizen>()
                .Property(c => c.TaxId).HasMaxLength(11);
            modelBuilder.Entity<Citizen>()
                .HasMany(c => c.Applications)
                .WithOne(a => a.Citizen)
                .HasForeignKey(a => a.CitizenId)
                .IsRequired();

            // PublicOffice configuration
            modelBuilder.Entity<PublicOffice>()
                .HasKey(o => o.OfficeId);
            modelBuilder.Entity<PublicOffice>()
                .Property(o => o.OfficeName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<PublicOffice>()
                .Property(o => o.DistrictMultiplier).HasPrecision(5, 2);
            modelBuilder.Entity<PublicOffice>()
                .HasMany(o => o.Applications)
                .WithOne(a => a.Office)
                .HasForeignKey(a => a.OfficeId)
                .IsRequired();

            // ServiceType configuration
            modelBuilder.Entity<ServiceType>()
                .HasKey(s => s.ServiceTypeId);
            modelBuilder.Entity<ServiceType>()
                .Property(s => s.ServiceName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<ServiceType>()
                .Property(s => s.BaseFee).HasPrecision(10, 2);
            modelBuilder.Entity<ServiceType>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.ServiceType)
                .HasForeignKey(a => a.ServiceTypeId)
                .IsRequired();

            // ServiceApplication configuration
            modelBuilder.Entity<ServiceApplication>()
                .HasKey(a => a.ApplicationId);
            modelBuilder.Entity<ServiceApplication>()
                .Property(a => a.ApplicationNumber).HasMaxLength(20);
            modelBuilder.Entity<ServiceApplication>()
                .Property(a => a.CalculatedFee).HasPrecision(10, 2);
            modelBuilder.Entity<ServiceApplication>()
                .HasMany(a => a.Documents)
                .WithOne(d => d.Application)
                .HasForeignKey(d => d.ApplicationId)
                .IsRequired();
            modelBuilder.Entity<ServiceApplication>()
                .HasMany(a => a.AuditLogs)
                .WithOne(l => l.Application)
                .HasForeignKey(l => l.ApplicationId)
                .IsRequired();

            // ApplicationDocument configuration
            modelBuilder.Entity<ApplicationDocument>()
                .HasKey(d => d.DocumentId);
            modelBuilder.Entity<ApplicationDocument>()
                .Property(d => d.DocumentName).IsRequired().HasMaxLength(200);

            // AuditLog configuration
            modelBuilder.Entity<AuditLog>()
                .HasKey(l => l.AuditLogId);
            modelBuilder.Entity<AuditLog>()
                .Property(l => l.Action).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<AuditLog>()
                .Property(l => l.PerformedBy).IsRequired().HasMaxLength(100);

            // FeeSchedule configuration
            modelBuilder.Entity<FeeSchedule>()
                .HasKey(f => f.FeeScheduleId);
            modelBuilder.Entity<FeeSchedule>()
                .Property(f => f.AdjustedBaseFee).HasPrecision(10, 2);
            modelBuilder.Entity<FeeSchedule>()
                .Property(f => f.DistrictCode).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<FeeSchedule>()
                .HasOne(f => f.ServiceType)
                .WithMany(s => s.FeeSchedules)
                .HasForeignKey(f => f.ServiceTypeId)
                .IsRequired();

            // Table name mappings
            modelBuilder.Entity<Citizen>().ToTable("Citizens");
            modelBuilder.Entity<PublicOffice>().ToTable("PublicOffices");
            modelBuilder.Entity<ServiceType>().ToTable("ServiceTypes");
            modelBuilder.Entity<ServiceApplication>().ToTable("ServiceApplications");
            modelBuilder.Entity<ApplicationDocument>().ToTable("ApplicationDocuments");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            modelBuilder.Entity<FeeSchedule>().ToTable("FeeSchedules");

            base.OnModelCreating(modelBuilder);
        }
    }
}
