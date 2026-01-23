using System;

namespace BookMyShow.Application.DTOs
{
    public class MovieShowAvailabilityDto
    {
        public Guid MovieShowId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int HeldSeats { get; set; }
        public int BookedSeats { get; set; }
    }
}