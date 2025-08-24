using Microsoft.EntityFrameworkCore;
using KitchenEquipmentManagement.Models;

namespace KitchenEquipmentManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<RegisteredEquipment> RegisteredEquipment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.UserType).HasColumnName("user_type").HasMaxLength(50);
                entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
                entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
                entity.Property(e => e.EmailAddress).HasColumnName("email_address").HasMaxLength(255);
                entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100);
                entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(255);
                entity.Property(e => e.CreatedDate).HasColumnName("created_date").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasColumnName("modified_date").HasDefaultValueSql("GETDATE()");

                // Unique constraints
                entity.HasIndex(e => e.EmailAddress).IsUnique();
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            // Site configuration
            modelBuilder.Entity<Site>(entity =>
            {
                entity.ToTable("Sites");
                entity.HasKey(e => e.SiteId);
                entity.Property(e => e.SiteId).HasColumnName("site_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
                entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasColumnName("created_date").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasColumnName("modified_date").HasDefaultValueSql("GETDATE()");

                // Foreign key relationship
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sites)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Equipment configuration
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipment");
                entity.HasKey(e => e.EquipmentId);
                entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
                entity.Property(e => e.SerialNumber).HasColumnName("serial_number").HasMaxLength(100);
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
                entity.Property(e => e.Condition).HasColumnName("condition").HasMaxLength(50);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasColumnName("modified_date").HasDefaultValueSql("GETDATE()");

                // Unique constraint for serial number
                entity.HasIndex(e => e.SerialNumber).IsUnique();

                // Foreign key relationship
                entity.HasOne(eq => eq.User)
                      .WithMany(u => u.Equipment)
                      .HasForeignKey(eq => eq.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // RegisteredEquipment configuration
            modelBuilder.Entity<RegisteredEquipment>(entity =>
            {
                entity.ToTable("RegisteredEquipment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
                entity.Property(e => e.SiteId).HasColumnName("site_id");
                entity.Property(e => e.RegisteredDate).HasColumnName("registered_date").HasDefaultValueSql("GETDATE()");

                // Unique constraint - one equipment can only be at one site
                entity.HasIndex(e => e.EquipmentId).IsUnique();

                // Composite unique constraint for safety
                entity.HasIndex(e => new { e.EquipmentId, e.SiteId }).IsUnique();

                // Foreign key relationships - Using NoAction to avoid cascade path conflicts
                entity.HasOne(re => re.Equipment)
                      .WithMany(eq => eq.RegisteredEquipments)
                      .HasForeignKey(re => re.EquipmentId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(re => re.Site)
                      .WithMany(s => s.RegisteredEquipments)
                      .HasForeignKey(re => re.SiteId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure custom delete behavior to handle cascade deletes manually
            // This ensures proper cleanup when users, sites, or equipment are deleted
        }

        public override int SaveChanges()
        {
            HandleCustomCascadeDeletes();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleCustomCascadeDeletes();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void HandleCustomCascadeDeletes()
        {
            var deletedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                // Handle Site deletion - remove all equipment assignments from this site
                if (entry.Entity is Site site)
                {
                    var equipmentAssignments = RegisteredEquipment
                        .Where(re => re.SiteId == site.SiteId)
                        .ToList();

                    RegisteredEquipment.RemoveRange(equipmentAssignments);
                }

                // Handle Equipment deletion - remove site assignment for this equipment
                if (entry.Entity is Equipment equipment)
                {
                    var equipmentAssignments = RegisteredEquipment
                        .Where(re => re.EquipmentId == equipment.EquipmentId)
                        .ToList();

                    RegisteredEquipment.RemoveRange(equipmentAssignments);
                }

                // Handle User deletion - this will cascade naturally for Sites and Equipment
                // but we need to clean up RegisteredEquipment entries
                if (entry.Entity is User user)
                {
                    // Get all sites and equipment for this user
                    var userSiteIds = Sites.Where(s => s.UserId == user.UserId).Select(s => s.SiteId).ToList();
                    var userEquipmentIds = Equipment.Where(e => e.UserId == user.UserId).Select(e => e.EquipmentId).ToList();

                    // Remove all registered equipment entries for this user's sites and equipment
                    var relatedAssignments = RegisteredEquipment
                        .Where(re => userSiteIds.Contains(re.SiteId) || userEquipmentIds.Contains(re.EquipmentId))
                        .ToList();

                    RegisteredEquipment.RemoveRange(relatedAssignments);
                }
            }
        }
    }
}