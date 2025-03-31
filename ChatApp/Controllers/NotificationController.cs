using System.Security.Claims;
using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Notifications()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Notification> notifications = await _notificationService.GetUserNotifications(userId);

            return View(notifications);
        }
    }
}
