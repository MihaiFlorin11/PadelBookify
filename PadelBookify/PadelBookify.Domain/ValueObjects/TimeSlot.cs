using PadelBookify.Domain.Exceptions;

namespace PadelBookify.Domain.ValueObjects
{
    public record TimeSlot
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        private TimeSlot() { } // pentru EF

        public TimeSlot(DateTime start, DateTime end)
        {
            if (start.Kind != DateTimeKind.Utc || end.Kind != DateTimeKind.Utc)
                throw new DomainException("Times must be UTC.");
            if (end <= start)
                throw new DomainException("End must be after start.");

            Start = start;
            End = end;
        }

        public TimeSpan Duration => End - Start;

        public bool OverlapsWith(TimeSlot other) =>
            Start < other.End && other.Start < End;
    }
}
