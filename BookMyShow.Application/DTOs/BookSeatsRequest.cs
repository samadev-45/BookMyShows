using System;
using System.Collections.Generic;

namespace BookMyShow.Application.DTOs
{
    public class BookSeatsRequest
    {
        public Guid MovieShowId { get; set; }
        public List<string> SeatNumbers { get; set; } = new List<string>();
        public Guid CorrelationId { get; set; }
    }
}