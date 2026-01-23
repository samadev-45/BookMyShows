using System;

namespace BookMyShow.Application.DTOs
{
    public class MovieShowSummaryDto
    {
        public Guid Id { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }
        public string ScreenNumber { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
    }
}