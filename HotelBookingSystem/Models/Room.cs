using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Type { get; set; }

        [Range(1, 10)]
        public int Capacity { get; set; }

        [Range(1, 10000)]
        public decimal PricePerNight { get; set; }

        [StringLength(500)]
        public required string Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
