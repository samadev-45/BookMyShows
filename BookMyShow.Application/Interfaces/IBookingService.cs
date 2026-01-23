using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookMyShow.Application.DTOs;

namespace BookMyShow.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookSeatsResponse> HoldSeatsAsync(BookSeatsRequest request);
        Task<bool> ConfirmBookingAsync(Guid bookingId);
        Task<bool> CancelBookingAsync(Guid bookingId);
        Task<MovieShowAvailabilityDto> GetMovieShowAvailabilityAsync(Guid movieShowId);
        Task<IEnumerable<MovieShowSummaryDto>> GetAllMovieShowsAsync();
        Task ReleaseExpiredHeldSeatsAsync();
    }
}