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
                entity.HasKey(e => e.DeviceId);
                entity.Property(e => e.DeviceName).IsRequired();
                entity.Property(e => e.Enabled).IsRequired();
                entity.Property(e => e.Pins);
                entity.HasOne(d => d.Gateway)
                    .WithMany(p => p.Devices);
                entity.HasOne(d => d.DeviceType)
                    .WithMany(p => p.Devices);
            });

            modelBuilder.Entity<Gateway>(entity =>
            {
                entity.HasKey(e => e.GatewayId);
                entity.HasOne(d => d.Room)
                  .WithMany(p => p.Gateways);
                entity.Property(e => e.Online);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomId);
                entity.Property(e => e.RoomName).IsRequired();
            });

            modelBuilder.Entity<DeviceType>(entity =>
            {
                entity.HasKey(e => e.DeviceTypeId);
                entity.Property(e => e.DeviceTypeName);
                entity.Property(e => e.Icon);
                entity.Property(e => e.Unit);
                entity.Property(e => e.Sensor);
            });
        }
    }
}
