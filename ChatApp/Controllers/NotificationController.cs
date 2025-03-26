using System.Security.Claims;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public NotificationController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IActionResult Notifications()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Notification> notifications = _dbContext.Notifications.Where(u=> u.UserId == userId)
                .GroupBy(u => u.Id)
                .Select(g => g.OrderByDescending(m => m.Date).FirstOrDefault()).ToList();
            foreach(var notification in notifications) 
            {
                notification.isRead = true;
            }
            _dbContext.SaveChanges();
            return View(notifications);
        }
    }
}
