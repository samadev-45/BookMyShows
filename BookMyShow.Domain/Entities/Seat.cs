using System;

namespace BookMyShow.Domain.Entities
{
    public class Seat
    {
        public Guid Id { get; set; }
        public Guid MovieShowId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public Enums.SeatStatus Status { get; set; }
        public Guid? BookingId { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}