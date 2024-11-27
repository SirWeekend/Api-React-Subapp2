using System.ComponentModel.DataAnnotations;

namespace Eksamen2024.Models
{
    public class Admin
    {
        [Key] // Marking AdminId as primary key
        public int AdminId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set;} = string.Empty;

        // Navigation for pinpoint used by the user, can be null
        public virtual ICollection<Pinpoint>? Pinpoint { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}