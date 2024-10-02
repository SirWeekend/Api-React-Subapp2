using System.ComponentModel.DataAnnotations;


namespace Eksamen2024.Models
{
    public class Comment
    {
        // Every comment gets an ID integer as primary key
        [Key]
        public int CommentId { get; set; }
        // Comment cannot be empty
        public string Text { get; set; } = string.Empty;

        // Foreign key to the pinpoint the comment is applied to
        public int PinpointId { get; set; }
        // Can be null if there are no comment
        public virtual Pinpoint? Pinpoint { get; set; }

        // Foreign key to the user that comments
        public int UserId { get; set; }
        // Can be null if there are no comments and no user
        public virtual Users? Users { get; set; }
    }
}