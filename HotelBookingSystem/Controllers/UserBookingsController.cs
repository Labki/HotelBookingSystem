using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Controllers
{
    [Authorize]  // users must be logged in
    public class UserBookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserBookingsController(
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        // GET: /UserBookings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _bookingService.GetBookingsForUserAsync(user.Id);
            return View(bookings);
        }

        // GET: /UserBookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null || booking.UserId != user.Id)
                return Unauthorized();

            return View(booking);
        }

        // GET: /UserBookings/Create
        public async Task<IActionResult> Create()
        {
            var rooms = await _bookingService.GetAllRoomsAsync();
            ViewBag.Rooms = rooms;

            return View();
        }

        // POST: /UserBookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _bookingService.CreateBookingAsync(user.Id, roomId, checkInDate, checkOutDate);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);

                var rooms = await _bookingService.GetAllRoomsAsync();
                ViewBag.Rooms = rooms;

                TempData["ToastMessage"] = result.ErrorMessage;

                return View();
            }
            TempData["ToastMessage"] = "Your booking was created successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null || booking.UserId != user.Id)
                return Unauthorized();

            var success = await _bookingService.UpdateBookingStatusAsync(id, BookingStatus.Cancelled);

            if (!success)
            {
                TempData["Error"] = "Could not cancel your booking.";
            }
            else
            {
                TempData["Success"] = "Your booking has been cancelled.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult GetFeaturesForType([FromBody] string roomType)
        {
            var features = RoomFeatureLookup.FeaturesByType[roomType];
            return PartialView("_RoomFeaturesPartial", features);
        }
    }
}
