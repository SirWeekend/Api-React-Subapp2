using System.ComponentModel.DataAnnotations;


namespace Eksamen2024.Models
{
    public class User
    {
        [Key] // Marking UserId as primary key
        public int UserId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string HashedPassword { get; set;} = string.Empty;

        // Navigation for pinpoint used by the user, can be null
        public virtual ICollection<Pinpoint>? Pinpoint { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}