using System.ComponentModel.DataAnnotations;


namespace Eksamen2024.Models
{
    public class Pinpoint
    {
        // Every pinpoint gets an Id and is the primary key
        [Key]
        public int PinpointId { get; set; }
        // A pinpoint name cant be empty
        public string Name { get; set; } = string.Empty;
        // Using double for more precise measurements of location
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Description and imageUrl does not need to be filled out
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Foreign key to the user for the pinpoint
        public int UserId { get; set; }
        // Can be null if there are no users
        public Users? Users { get; set; }

        // Navigation for the user comments, can be null if there are no comments
        public List<Comment>? Comments { get; set;}
    }
}