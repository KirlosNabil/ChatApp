using ChatApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;

namespace ChatApp.ViewComponents
{
    public class FriendRequestCounterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FriendRequestCounterViewComponent(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int friendRequestCount = await GetFriendRequestCount();
            return View(friendRequestCount);
        }

        private async Task<int> GetFriendRequestCount()
        {
            string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            int pendingRequestsCount = await _dbContext.FriendRequests
                .CountAsync(u => u.ReceiverId == userId
                             && u.Status == FriendRequestStatus.Pending);

            return pendingRequestsCount;
        }
    }
}
