using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .OrderByDescending(b => b.CheckInDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsForUserAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CheckInDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateBookingAsync(
            string userId, int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn.Date < DateTime.Today)
                return (false, "Check-in date must be today or later.");

            if (checkOut <= checkIn)
                return (false, "Check-out must be after check-in.");

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return (false, "Room not found.");

            // check overlap
            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Completed &&
                (
                    (checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                    (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                    (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate)
                )
            );

            if (overlap)
                return (false, "Room is not available for chosen dates.");

            var nights = (checkOut - checkIn).Days;
            var booking = new Booking
            {
                RoomId = roomId,
                UserId = userId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalPrice = nights * room.PricePerNight,
                Status = BookingStatus.Pending
            };

            _context.Add(booking);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            if (!await _context.Bookings.AnyAsync(b => b.Id == booking.Id))
                return false;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room?> GetRoomAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }
        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return false;

            booking.Status = newStatus;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CreateGuestUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result.Succeeded;
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateManualBookingAsync(Booking booking)
        {
            if (booking.CheckOutDate <= booking.CheckInDate)
                return (false, "Check-out date must be after check-in date.");

            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room == null)
                return (false, "Room not found.");

            // Overlap check
            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Completed &&
                (
                    (booking.CheckInDate >= b.CheckInDate && booking.CheckInDate < b.CheckOutDate) ||
                    (booking.CheckOutDate > b.CheckInDate && booking.CheckOutDate <= b.CheckOutDate) ||
                    (booking.CheckInDate <= b.CheckInDate && booking.CheckOutDate >= b.CheckOutDate)
                )
            );

            if (overlap)
                return (false, "Room is not available for the selected dates.");

            var nights = (booking.CheckOutDate - booking.CheckInDate).Days;

            booking.TotalPrice = nights * room.PricePerNight;

            // Manual bookings from admin are CONFIRMED
            if (booking.Status == 0)
                booking.Status = BookingStatus.Confirmed;

            // UserId is NULL 
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
