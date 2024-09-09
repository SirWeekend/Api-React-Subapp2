using System.ComponentModel.DataAnnotations;


namespace Eksamen2024.Models
{
    public class Users
    {
        [Key] // Marking UserId as primary key
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set;} = string.Empty;

        // Navigation for pinpoint used by the user, can be null
        public List<Pinpoint>? Pinpoint { get; set; }
    }
}