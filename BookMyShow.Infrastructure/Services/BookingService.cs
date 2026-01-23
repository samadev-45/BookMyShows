using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookMyShow.Application.Interfaces;
using BookMyShow.Application.DTOs;
using BookMyShow.Domain.Entities;
using BookMyShow.Domain.Enums;
using BookMyShow.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace BookMyShow.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BookSeatsResponse> HoldSeatsAsync(BookSeatsRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var movieShow = await _context.MovieShows
                    .FirstOrDefaultAsync(ms => ms.Id == request.MovieShowId);

                if (movieShow == null)
                {
                    return new BookSeatsResponse { Success = false, Message = "Movie show not found." };
                }

              // Pessimistic locking for seat selection
                // For SQL Server, use UPDLOCK, ROWLOCK, READPAST hints
                // Using parameterized query to prevent SQL injection and integrating status check directly into SQL.

         
                var seatNumberParams = request.SeatNumbers.Select((sn, index) => new SqlParameter($"@p_seat_{index}", sn)).ToArray();
                var inClause = string.Join(", ", seatNumberParams.Select(p => p.ParameterName));

                var sql = $"SELECT * FROM Seats WITH (UPDLOCK, ROWLOCK, READPAST) WHERE MovieShowId = @p_movieShowId AND Status = @p_status AND SeatNumber IN ({inClause})";

               
                var allParams = new List<object> 
                {
                    new SqlParameter("@p_movieShowId", request.MovieShowId),
                    new SqlParameter("@p_status", SeatStatus.Available.ToString())
                };
                allParams.AddRange(seatNumberParams);

                var availableSeats = await _context.Seats
                    .FromSqlRaw(sql, allParams.ToArray())
                    .ToListAsync();

                if (availableSeats.Count != request.SeatNumbers.Count)
                {
                    await transaction.RollbackAsync();
                    return new BookSeatsResponse { Success = false, Message = "Some selected seats are no longer available or are held by another user." };
                }

                var bookingId = Guid.NewGuid();
                var expiryTime = DateTime.UtcNow.AddMinutes(1); 

                foreach (var seat in availableSeats)
                {
                    seat.Status = SeatStatus.Held;
                    seat.BookingId = bookingId;
                    seat.ExpiryTime = expiryTime;
                }

                var booking = new Booking
                {
                    Id = bookingId,
                    CorrelationId = request.CorrelationId,
                    MovieShowId = request.MovieShowId,
                    BookingStatus = BookingStatus.Pending,
                    BookingTime = DateTime.UtcNow,
                    ExpiryTime = expiryTime
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BookSeatsResponse { Success = true, BookingId = bookingId, Message = "Seats held successfully." };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict occurred during seat holding. Please try again.");
                return new BookSeatsResponse { Success = false, Message = "Concurrency conflict occurred during seat holding. Please try again." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while holding seats.");
                return new BookSeatsResponse { Success = false, Message = "An error occurred while holding seats." };
            }
        }

        public async Task<bool> ConfirmBookingAsync(Guid bookingId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                // if booking is pending AND not expired
                if (booking == null || booking.BookingStatus != BookingStatus.Pending || booking.ExpiryTime <= DateTime.UtcNow)
                {
                    await transaction.RollbackAsync();
                    // update booking status to expired here if it's past expiry but still pending
                    if (booking != null && booking.BookingStatus == BookingStatus.Pending && booking.ExpiryTime <= DateTime.UtcNow)
                    {
                        booking.BookingStatus = BookingStatus.Expired;
                        await _context.SaveChangesAsync();
                    }
                    return false; 
                }

                var seats = await _context.Seats
                    .Where(s => s.BookingId == bookingId && s.Status == SeatStatus.Held)
                    .ToListAsync();

                if (!seats.Any())
                {
                    await transaction.RollbackAsync();
                    return false; 
                }

                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Booked;
                }

                booking.BookingStatus = BookingStatus.Confirmed;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict occurred while confirming booking.");
                return false; // Concurrency conflict
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while confirming booking.");
                return false;
            }
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null || booking.BookingStatus != BookingStatus.Pending)
                {
                    await transaction.RollbackAsync();
                    return false; // Booking not found or not in pending state
                }

                var seats = await _context.Seats
                    .Where(s => s.BookingId == bookingId && s.Status == SeatStatus.Held)
                    .ToListAsync();

                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Available;
                    seat.BookingId = null;
                    seat.ExpiryTime = null;
                }

                booking.BookingStatus = BookingStatus.Cancelled;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict occurred while cancelling booking.");
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while cancelling booking.");
                return false;
            }
        }

        public async Task<MovieShowAvailabilityDto> GetMovieShowAvailabilityAsync(Guid movieShowId)
        {
            var totalSeats = await _context.MovieShows
                .Where(ms => ms.Id == movieShowId)
                .Select(ms => ms.TotalSeats)
                .FirstOrDefaultAsync();

            if (totalSeats == 0)
            {
                return new MovieShowAvailabilityDto { MovieShowId = movieShowId, TotalSeats = 0, AvailableSeats = 0, HeldSeats = 0, BookedSeats = 0 };
            }

            var seats = await _context.Seats
                .Where(s => s.MovieShowId == movieShowId)
                .ToListAsync();

            var availableSeats = seats.Count(s => s.Status == SeatStatus.Available);
            var heldSeats = seats.Count(s => s.Status == SeatStatus.Held);
            var bookedSeats = seats.Count(s => s.Status == SeatStatus.Booked);

            return new MovieShowAvailabilityDto
            {
                MovieShowId = movieShowId,
                TotalSeats = totalSeats,
                AvailableSeats = availableSeats,
                HeldSeats = heldSeats,
                BookedSeats = bookedSeats
            };
        }

        public async Task ReleaseExpiredHeldSeatsAsync()
        {
            var now = DateTime.UtcNow;
            // Find expired held seats, potentially acquire locks to prevent race conditions with user confirming at the same time
            var expiredHeldSeats = await _context.Seats
                .Where(s => s.Status == SeatStatus.Held && s.ExpiryTime != null && s.ExpiryTime <= now)
                .ToListAsync();

            if (!expiredHeldSeats.Any()) return;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bookingIdsToCancel = expiredHeldSeats.Where(s => s.BookingId.HasValue).Select(s => s.BookingId!.Value).Distinct().ToList();
                var bookingsToCancel = await _context.Bookings
                    .Where(b => bookingIdsToCancel.Contains(b.Id) && b.BookingStatus == BookingStatus.Pending)
                    .ToListAsync();

                foreach (var seat in expiredHeldSeats)
                {
                    seat.Status = SeatStatus.Available;
                    seat.BookingId = null;
                    seat.ExpiryTime = null;
                }

                foreach (var booking in bookingsToCancel)
                {
                    booking.BookingStatus = BookingStatus.Expired;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict while releasing expired seats. Retrying on next cycle.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during expired seat release.");
            }
        }

        public async Task<IEnumerable<MovieShowSummaryDto>> GetAllMovieShowsAsync()
        {
            return await _context.MovieShows
                .Select(ms => new MovieShowSummaryDto
                {
                    Id = ms.Id,
                    MovieTitle = ms.MovieTitle,
                    ShowTime = ms.ShowTime,
                    ScreenNumber = ms.ScreenNumber,
                    TotalSeats = ms.TotalSeats
                })
                .ToListAsync();
        }
    }
}