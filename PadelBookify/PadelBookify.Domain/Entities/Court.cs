using PadelBookify.Domain.Exceptions;
using PadelBookify.Domain.ValueObjects;

namespace PadelBookify.Domain.Entities
{
    public class Court
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public decimal PricePerHour { get; private set; }
        public TimeOnly OpensAt { get; private set; }
        public TimeOnly ClosesAt { get; private set; }
        public bool IsActive { get; private set; }

        private Court() { }

        public static Court Create(string name, decimal pricePerHour,
                                   TimeOnly opensAt, TimeOnly closesAt)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");
            if (pricePerHour <= 0)
                throw new DomainException("Price must be positive.");
            if (closesAt <= opensAt)
                throw new DomainException("Closing time must be after opening time.");

            return new Court
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                PricePerHour = pricePerHour,
                OpensAt = opensAt,
                ClosesAt = closesAt,
                IsActive = true
            };
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new DomainException("Price must be positive.");
            PricePerHour = newPrice;
        }

        public bool IsOpenDuring(TimeSlot slot) =>
            TimeOnly.FromDateTime(slot.Start) >= OpensAt &&
            TimeOnly.FromDateTime(slot.End) <= ClosesAt &&
            slot.Start.Date == slot.End.Date;

        public decimal CalculatePrice(TimeSlot slot) =>
            PricePerHour * (decimal)slot.Duration.TotalHours;
    }
}
