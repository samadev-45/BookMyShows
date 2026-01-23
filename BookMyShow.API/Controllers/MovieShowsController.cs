using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookMyShow.Application.Interfaces;
using BookMyShow.Application.DTOs;

namespace BookMyShow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieShowsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public MovieShowsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovieShowSummaryDto>), 200)]
        public async Task<IActionResult> GetAllMovieShows()
        {
            var shows = await _bookingService.GetAllMovieShowsAsync();
            return Ok(shows);
        }

        [HttpGet("{id}/availability")]
        [ProducesResponseType(typeof(MovieShowAvailabilityDto), 200)]
     
        public async Task<IActionResult> GetAvailability(Guid id)
        {
            var availability = await _bookingService.GetMovieShowAvailabilityAsync(id);
            if (availability.MovieShowId == Guid.Empty) 
            {
                return NotFound("Movie show not found.");
            }
            return Ok(availability);
        }

        [HttpPost("{id}/hold-seats")]
        [ProducesResponseType(typeof(BookSeatsResponse), 200)]
  
        public async Task<IActionResult> HoldSeats(Guid id, [FromBody] BookSeatsRequest request)
        {
            if (id != request.MovieShowId)
            {
                return BadRequest("MovieShowId in URL does not match body.");
            }

            var response = await _bookingService.HoldSeatsAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("bookings/{bookingId}/confirm")]
        [ProducesResponseType(200)]
        
        public async Task<IActionResult> ConfirmBooking(Guid bookingId)
        {
            var result = await _bookingService.ConfirmBookingAsync(bookingId);
            if (!result)
            {
                return BadRequest("Booking could not be confirmed. It might be already confirmed, expired, or not found.");
            }
            return Ok("Booking confirmed successfully.");
        }

        [HttpPost("bookings/{bookingId}/cancel")]
        [ProducesResponseType(200)]
       
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);
            if (!result)
            {
                return BadRequest("Booking could not be cancelled. Only pending bookings can be cancelled, or booking not found.");
            }
            return Ok("Booking cancelled successfully.");
        }
    }
}