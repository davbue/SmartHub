using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace SmartHub.Models
{
    public class SmartHomeContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=SmartHub;user=root;password=");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.DeviceID);
                entity.Property(e => e.DeviceName).IsRequired();
                entity.Property(e => e.Enabled).IsRequired();
                entity.HasOne(d => d.Gateway)
                    .WithMany(p => p.Devices);
                entity.HasOne(d => d.DeviceType)
                    .WithMany(p => p.Devices);
            });

            modelBuilder.Entity<Gateway>(entity =>
            {
                entity.HasKey(e => e.GatewayID);
                entity.HasOne(d => d.Room)
                  .WithMany(p => p.Gateways);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomID);
                entity.Property(e => e.RoomName).IsRequired();
            });

            modelBuilder.Entity<DeviceType>(entity =>
            {
                entity.HasKey(e => e.DeviceTypeID);
                entity.Property(e => e.DeviceTypeName);
            });
        }
    }
}
