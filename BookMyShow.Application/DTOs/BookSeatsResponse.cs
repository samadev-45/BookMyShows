using System;

namespace BookMyShow.Application.DTOs
{
    public class BookSeatsResponse
    {
        public Guid BookingId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}