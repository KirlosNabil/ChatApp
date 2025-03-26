using ChatApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.ViewComponents
{
    public class NotificationCounterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationCounterViewComponent(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int notificationCount = await GetNotificationCount();
            return View(notificationCount);
        }

        private async Task<int> GetNotificationCount()
        {
            string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            int notificationCount = await _dbContext.Notifications
                .CountAsync(u => u.UserId == userId
                             && u.isRead == false);

            return notificationCount;
        }
    }
}
