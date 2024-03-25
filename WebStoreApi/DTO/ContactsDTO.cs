using System.ComponentModel.DataAnnotations;
using WebStoreApi.Models;

namespace WebStoreApi.DTO
{
    public class ContactsDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required,EmailAddress,MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string? Phone { get; set; }
        [Required]
        public Guid SubjectId { get; set; }
        [Required,MinLength(20),MaxLength(4000)]
        public string Message { get; set; }
    }
}
