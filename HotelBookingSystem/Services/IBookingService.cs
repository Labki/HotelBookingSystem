using HotelBookingSystem.Models;

namespace HotelBookingSystem.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetBookingsForUserAsync(string userId);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateBookingAsync(string userId, int roomId, DateTime checkIn, DateTime checkOut);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

        // Rooms
        Task<List<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomAsync(int id);

        // ⭐ NEW: status update for admin
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus);
    }
}
