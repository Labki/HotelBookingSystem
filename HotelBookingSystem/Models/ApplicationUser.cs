using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
