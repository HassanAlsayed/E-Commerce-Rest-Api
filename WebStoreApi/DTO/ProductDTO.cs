using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebStoreApi.DTO
{
    public class ProductDTO
    {
        [Required,MaxLength(100)]
        public string Name { get; set; }
        [Required,MaxLength(100)]
        public string Description { get; set; }
        [Required,MaxLength(100)]
        public string Category { get; set; }
        [Required,MaxLength(100)]
        public string Brand { get; set; }
        [Required]
        public double Price { get; set; }
        [NotMapped]
        public IFormFile ImageFileName { get; set; }
        [HiddenInput]
        public string? ImageUrl { get; set; }
    }
}
