using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RoomsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms.ToListAsync());
        }

        // GET: Admin/Rooms/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        // GET: Admin/Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(room);

            // Upload image if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images", "rooms");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                room.ImageUrl = "/images/rooms/" + fileName;
            }

            _context.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Rooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room updatedRoom, IFormFile? imageFile)
        {
            if (id != updatedRoom.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updatedRoom);

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
                return NotFound();

            // Update base properties
            existingRoom.Name = updatedRoom.Name;
            existingRoom.Type = updatedRoom.Type;
            existingRoom.Capacity = updatedRoom.Capacity;
            existingRoom.PricePerNight = updatedRoom.PricePerNight;
            existingRoom.Description = updatedRoom.Description;

            // Handle image upload if new image provided
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images", "rooms");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingRoom.ImageUrl = "/images/rooms/" + fileName;
            }

            _context.Update(existingRoom);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Rooms/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Admin/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
