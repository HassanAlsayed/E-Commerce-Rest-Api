using System.ComponentModel.DataAnnotations;

namespace WebStoreApi.Models
{
    public class Contact
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(100)]
        public string Phone { get; set; }
        [Required]
        public required Subject Subjects { get; set; }
        [MaxLength(100)]
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

       

    }
}
