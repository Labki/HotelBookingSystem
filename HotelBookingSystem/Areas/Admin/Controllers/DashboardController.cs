using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalRooms = await _context.Rooms.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var pendingBookings = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Pending);

            var confirmedToday = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Confirmed &&
                                 b.CheckInDate.Date == DateTime.Today);

            // Latest 5 bookings for dashboard preview
            var latestBookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .OrderByDescending(b => b.CheckInDate)
                .Take(5)
                .ToListAsync();

            var calendar = new BookingCalendarViewModel
            {
                Rooms = await _context.Rooms.ToListAsync(),
                Bookings = await _context.Bookings
                 .Include(b => b.User)
                 .Include(b => b.Room)
                 .ToListAsync(),
                Dates = Enumerable.Range(0, 30)
             .Select(i => DateTime.Today.AddDays(i))
             .ToList()
            };

            var vm = new AdminDashboardViewModel
            {
                TotalRooms = totalRooms,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                TodayCheckIns = confirmedToday,
                LatestBookings = latestBookings,
                Calendar = calendar
            };

            return View(vm);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int TodayCheckIns { get; set; }
        public List<Booking> LatestBookings { get; set; } = new();
        public BookingCalendarViewModel Calendar { get; set; }
    }
}
