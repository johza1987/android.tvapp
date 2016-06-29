using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Hotspot> Hotspot { get; set; }
        public DbSet<HotspotUser> HotspotUser { get; set; }
        public DbSet<HotspotPlan> HotspotPlan { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<BatchUser> BatchUser { get; set; }
        public DbSet<Userinfo> Userinfo { get; set; }
        public DbSet<UserBillinfo> UserBillinfo { get; set; }
        public DbSet<RadCheck> RadCheck { get; set; }
        public DbSet<RadUsergroup> RadUsergroup { get; set; }
        public DbSet<RadAcct> RadAcct { get; set; }
        public DbSet<GenerateCode> GenerateCode { get; set; }
        public DbSet<Operator> Operator { get; set; }

        public DatabaseContext()
            : base("DefaultConnection")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BatchUser>().Ignore(t => t.packageId);
            modelBuilder.Entity<BatchUser>().Ignore(t => t.quantity);

            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Composite primary key 
            //modelBuilder.Entity<HotspotUser>().HasKey(u => new { u.id , u.code });

            //Composite foreign key 
            //modelBuilder.Entity<HotspotUser>()
            //    .HasRequired(h => h.Hotspot)
            //    .WithMany(u => u.HotspotUser)
            //    .HasForeignKey(u => new { u.code });

            //modelBuilder.ComplexType<RadUsergroup>();
        }
    }
}