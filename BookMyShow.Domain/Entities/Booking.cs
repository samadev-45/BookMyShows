using System;

namespace BookMyShow.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid MovieShowId { get; set; }
        public Enums.BookingStatus BookingStatus { get; set; }
        public DateTime BookingTime { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}