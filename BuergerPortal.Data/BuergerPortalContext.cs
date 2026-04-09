using System.Data.Entity;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Data
{
    public class BuergerPortalContext : DbContext
    {
        public BuergerPortalContext()
            : base("Name=BuergerPortalContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<PublicOffice> PublicOffices { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<ServiceApplication> ServiceApplications { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<FeeSchedule> FeeSchedules { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
                .WithRequired(a => a.Citizen)
                .HasForeignKey(a => a.CitizenId);

            // PublicOffice configuration
            modelBuilder.Entity<PublicOffice>()
                .HasKey(o => o.OfficeId);
            modelBuilder.Entity<PublicOffice>()
                .Property(o => o.OfficeName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<PublicOffice>()
                .Property(o => o.DistrictMultiplier).HasPrecision(5, 2);
            modelBuilder.Entity<PublicOffice>()
                .HasMany(o => o.Applications)
                .WithRequired(a => a.Office)
                .HasForeignKey(a => a.OfficeId);

            // ServiceType configuration
            modelBuilder.Entity<ServiceType>()
                .HasKey(s => s.ServiceTypeId);
            modelBuilder.Entity<ServiceType>()
                .Property(s => s.ServiceName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<ServiceType>()
                .Property(s => s.BaseFee).HasPrecision(10, 2);
            modelBuilder.Entity<ServiceType>()
                .HasMany(s => s.Applications)
                .WithRequired(a => a.ServiceType)
                .HasForeignKey(a => a.ServiceTypeId);

            // ServiceApplication configuration
            modelBuilder.Entity<ServiceApplication>()
                .HasKey(a => a.ApplicationId);
            modelBuilder.Entity<ServiceApplication>()
                .Property(a => a.ApplicationNumber).HasMaxLength(20);
            modelBuilder.Entity<ServiceApplication>()
                .Property(a => a.CalculatedFee).HasPrecision(10, 2);
            modelBuilder.Entity<ServiceApplication>()
                .HasMany(a => a.Documents)
                .WithRequired(d => d.Application)
                .HasForeignKey(d => d.ApplicationId);
            modelBuilder.Entity<ServiceApplication>()
                .HasMany(a => a.AuditLogs)
                .WithRequired(l => l.Application)
                .HasForeignKey(l => l.ApplicationId);

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
                .HasRequired(f => f.ServiceType)
                .WithMany(s => s.FeeSchedules)
                .HasForeignKey(f => f.ServiceTypeId);

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
