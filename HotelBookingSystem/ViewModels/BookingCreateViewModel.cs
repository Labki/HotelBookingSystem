using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels.Bookings
{
    public class BookingCreateViewModel
    {
        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out date")]
        public DateTime CheckOutDate { get; set; }

        // For dropdown in the view
        public IEnumerable<SelectListItem> Rooms { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
