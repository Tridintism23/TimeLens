using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public PhilosophyType? PreferredPhilosophy {  get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<JournalEntry> JournalEntries { get; private set; } = new List<JournalEntry>();
        public ICollection<MoodEntry> MoodEntries { get; private set; } = new List<MoodEntry>();
        public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

        private User() { }

        // Factory Method pattern - Tạo User qua static method,
        // không cho phép new User() từ bên ngoài
        public static User Create(string email, string fullName, string passwordHash)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Email không được để trống.");
            }
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Tên không được để trống.");
            }
            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException("Password không được để trống.");
            }
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant().Trim(),
                FullName = fullName.Trim(),
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public void SetPreferredPhilosophy (PhilosophyType philosophy)
        {
            PreferredPhilosophy = philosophy;
            UpdatedAt = DateTime.UtcNow;
        }
        public void RecordLogin()
        {   
            LastLoginAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string fullName, string email)
        {
            if (string.IsNullOrWhiteSpace(fullName)) {
                throw new ArgumentException("Tên không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email không được để trống.");
            }
            FullName = fullName.Trim();
            Email = email.ToLowerInvariant().Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword (string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Password không được để trống.");
            }
            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeActivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
