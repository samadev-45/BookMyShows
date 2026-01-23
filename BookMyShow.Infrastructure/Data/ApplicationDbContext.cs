using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BookMyShow.Domain.Entities;
using BookMyShow.Domain.Enums;

namespace BookMyShow.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<MovieShow> MovieShows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Seat>()
                .Property(s => s.RowVersion)
                .IsRowVersion(); 

            modelBuilder.Entity<Seat>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // Configure Booking entity
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingStatus)
                .HasConversion<string>(); 

            modelBuilder.Entity<Booking>()
                .HasMany<Seat>() 
                .WithOne() 
                .HasForeignKey(s => s.BookingId)
                .IsRequired(false);

            // Fixed GUIDs for MovieShows for deterministic seeding
            var show1Id = Guid.Parse("67D35D72-2FDC-4C6E-A5B1-6A2EB7503C16");
            var show2Id = Guid.Parse("F525EBB5-623F-4779-BBAA-79557D2B909D");

            // Seed MovieShows
            modelBuilder.Entity<MovieShow>().HasData(
                new MovieShow
                {
                    Id = show1Id,
                    MovieTitle = "Spider-Man: No Way Home",
                    ShowTime = new DateTime(2026, 1, 24, 16, 56, 45, DateTimeKind.Utc),
                    ScreenNumber = "Screen 2",
                    TotalSeats = 75
                },
                new MovieShow
                {
                    Id = show2Id,
                    MovieTitle = "Avengers: Endgame",
                    ShowTime = new DateTime(2026, 1, 23, 16, 56, 45, DateTimeKind.Utc),
                    ScreenNumber = "Screen 1",
                    TotalSeats = 100
                }
            );

            // Seed Seats for each MovieShow
            var seats = new List<Seat>();
            string[] seatNumbers = { "A1", "A2", "A3", "B1", "B2", "C1", "C2", "C3", "D1", "D2" }; 

            foreach (var seatNumber in seatNumbers)
            {
                seats.Add(new Seat
                {
                    Id = Guid.NewGuid(),
                    MovieShowId = show1Id,
                    SeatNumber = seatNumber,
                    Status = SeatStatus.Available,
                    RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 } 
                });

                seats.Add(new Seat
                {
                    Id = Guid.NewGuid(),
                    MovieShowId = show2Id,
                    SeatNumber = seatNumber,
                    Status = SeatStatus.Available,
                    RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 } 
                });
            }
            modelBuilder.Entity<Seat>().HasData(seats);
        }
    }
}