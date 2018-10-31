using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMisWeb.Models;

namespace MyMisWeb.Data
{
    public class MyMisContext : DbContext
    {
        public MyMisContext(DbContextOptions<MyMisContext> options) : base(options)
        {
        }

        public DbSet<MisUser> MisUsers { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<ModifyRight> ModifyRights { get; set; }
        
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MisUser>().ToTable("MisUser").HasIndex(b=>b.IdentityName);
            modelBuilder.Entity<MisUser>().HasIndex(b => b.Deleted);
            modelBuilder.Entity<MisUser>().Property(b => b.Created).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<MisUser>().Property(b => b.Deleted).HasDefaultValue(false);

            modelBuilder.Entity<Center>().ToTable("Center").HasIndex(b=>b.Name);
            modelBuilder.Entity<Center>().HasIndex(b => b.Deleted);
            modelBuilder.Entity<Center>().HasIndex(b => b.DoNotTrackMonthStatus);
            modelBuilder.Entity<Center>().Property(b => b.Deleted).HasDefaultValue(false);
            modelBuilder.Entity<Center>().Property(b => b.DoNotTrackMonthStatus).HasDefaultValue(false);

            modelBuilder.Entity<Region>().ToTable("Region").HasIndex(b => b.Name);
            modelBuilder.Entity<Region>().HasIndex(b => b.Deleted);
            modelBuilder.Entity<Region>().Property(b => b.Deleted).HasDefaultValue(false);

            modelBuilder.Entity<ModifyRight>().ToTable("ModifyRight");

            modelBuilder.Entity<EmailTemplate>().ToTable("EmailTemplate").HasIndex(b=> b.IsForMonthStatusNotification);
        }
    }
}
