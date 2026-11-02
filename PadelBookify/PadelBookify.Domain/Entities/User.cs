using PadelBookify.Domain.Enums;
using PadelBookify.Domain.Exceptions;

namespace PadelBookify.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string FullName { get; private set; } = default!;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { }

        public static User Create(string email, string passwordHash,
                                  string fullName, UserRole role, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new DomainException("Invalid email.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name is required.");

            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                FullName = fullName.Trim(),
                Role = role,
                CreatedAt = now
            };
        }

        public void ChangePasswordHash(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash))
                throw new DomainException("Password hash is required.");
            PasswordHash = newHash;
        }
    }
}
