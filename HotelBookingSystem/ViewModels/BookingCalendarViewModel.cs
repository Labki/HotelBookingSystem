using HotelBookingSystem.Models;

public class BookingCalendarViewModel
{
    public List<Room> Rooms { get; set; } = new();
    public List<Booking> Bookings { get; set; } = new();
    public List<DateTime> Dates { get; set; } = new();
}
