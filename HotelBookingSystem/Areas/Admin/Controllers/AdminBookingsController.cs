using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminBookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public AdminBookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: Admin/AdminBookings
        public async Task<IActionResult> Index(
            int? roomId,
            BookingStatus? status,
            string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "date_asc";

            ViewBag.CurrentSort = sortOrder;

            var bookings = await _bookingService.GetAllBookingsAsync();
            var allRooms = await _bookingService.GetAllRoomsAsync();

            if (roomId.HasValue && roomId.Value > 0)
                bookings = bookings.Where(b => b.RoomId == roomId.Value).ToList();

            if (status.HasValue)
                bookings = bookings.Where(b => b.Status == status.Value).ToList();

            ViewBag.RoomSort = sortOrder == "room_asc" ? "room_desc" : "room_asc";
            ViewBag.DateSort = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            bookings = sortOrder switch
            {
                "room_asc" => bookings.OrderBy(b => b.Room.Name).ToList(),
                "room_desc" => bookings.OrderByDescending(b => b.Room.Name).ToList(),

                "date_asc" => bookings.OrderBy(b => b.CheckInDate).ToList(),
                "date_desc" => bookings.OrderByDescending(b => b.CheckInDate).ToList(),

                "name_asc" => bookings.OrderBy(b => b.User.FirstName).ToList(),
                "name_desc" => bookings.OrderByDescending(b => b.User.FirstName).ToList(),

                _ => bookings.OrderByDescending(b => b.CheckInDate).ToList()
            };

            // Provide data to dropdowns
            ViewBag.Rooms = allRooms;
            ViewBag.SelectedRoom = roomId;
            ViewBag.SelectedStatus = status;

            return View(bookings);
        }

        // GET: Admin/AdminBookings/Create
        public async Task<IActionResult> Create()
        {
            var rooms = await _bookingService.GetAllRoomsAsync();
            ViewBag.Rooms = rooms;

            return View();
        }

        // POST: Admin/AdminBookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking, string guestFullName)
        {
            ModelState.Remove("Room");
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = await _bookingService.GetAllRoomsAsync();
                return View(booking);
            }

            // Create temporary guest user
            var guestUser = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
                Email = $"{Guid.NewGuid()}@guest.local",
                FirstName = guestFullName.Split(" ").First(),
                LastName = guestFullName.Contains(" ") ? guestFullName.Split(" ").Last() : "Guest",
                DisplayName = guestFullName,
                EmailConfirmed = true
            };

            await _bookingService.CreateGuestUserAsync(guestUser);

            booking.UserId = guestUser.Id;

            var result = await _bookingService.CreateManualBookingAsync(booking);

            if (!result.Success)
            {
                ModelState.AddModelError("", "Unable to create booking.");
                ViewBag.Rooms = await _bookingService.GetAllRoomsAsync();
                return View(booking);
            }

            TempData["Success"] = "Booking created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminBookings/Details
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Admin/AdminBookings/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Admin/AdminBookings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(booking);

            var success = await _bookingService.UpdateBookingAsync(booking);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminBookings/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Admin/AdminBookings/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _bookingService.DeleteBookingAsync(id);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AdminBookings/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _bookingService.UpdateBookingStatusAsync(id, BookingStatus.Confirmed);

            if (!success)
            {
                TempData["Error"] = "Booking could not be approved.";
            }
            else
            {
                TempData["Success"] = "Booking approved successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AdminBookings/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _bookingService.UpdateBookingStatusAsync(id, BookingStatus.Cancelled);

            if (!success)
            {
                TempData["Error"] = "Booking could not be cancelled.";
            }
            else
            {
                TempData["Success"] = "Booking cancelled.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AdminBookings/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var success = await _bookingService.UpdateBookingStatusAsync(id, BookingStatus.Completed);

            if (!success)
            {
                TempData["Error"] = "Booking could not be marked as completed.";
            }
            else
            {
                TempData["Success"] = "Booking marked as completed.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
