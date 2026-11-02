using PadelBookify.Domain.Enums;
using PadelBookify.Domain.Exceptions;
using PadelBookify.Domain.ValueObjects;

namespace PadelBookify.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid CourtId { get; private set; }
        public Guid UserId { get; private set; }
        public TimeSlot Slot { get; private set; } = default!;
        public BookingStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        private Booking() { }

        public static Booking Create(Court court, Guid userId, TimeSlot slot, DateTime now)
        {
            if (!court.IsActive)
                throw new DomainException("Court is not available.");
            if (slot.Start <= now)
                throw new DomainException("Cannot book in the past.");
            if (slot.Duration < TimeSpan.FromMinutes(30) || slot.Duration > TimeSpan.FromHours(3))
                throw new DomainException("Booking must be between 30 minutes and 3 hours.");
            if (!court.IsOpenDuring(slot))
                throw new DomainException("Court is closed during the requested time.");

            return new Booking
            {
                Id = Guid.NewGuid(),
                CourtId = court.Id,
                UserId = userId,
                Slot = slot,
                Status = BookingStatus.Pending,
                TotalPrice = court.CalculatePrice(slot),
                CreatedAt = now
            };
        }

        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Only pending bookings can be confirmed.");
            Status = BookingStatus.Confirmed;
        }

        public void Cancel(DateTime now)
        {
            if (Status is BookingStatus.Cancelled or BookingStatus.Expired)
                throw new DomainException("Booking is already closed.");
            if (Slot.Start - now < TimeSpan.FromHours(24))
                throw new DomainException("Cannot cancel less than 24h before start.");

            Status = BookingStatus.Cancelled;
            CancelledAt = now;
        }

        public void Expire(DateTime now)
        {
            if (Status != BookingStatus.Pending)
                return; // idempotent: jobul poate rula de mai multe ori
            Status = BookingStatus.Expired;
        }

        public bool BlocksSlot => Status is BookingStatus.Pending or BookingStatus.Confirmed;
    }
}
