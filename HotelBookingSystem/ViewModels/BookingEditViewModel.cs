using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels.Bookings
{
    public class BookingEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Total price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Status")]
        public BookingStatus Status { get; set; }

        public IEnumerable<SelectListItem> Rooms { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
