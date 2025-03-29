using ChatApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.ViewComponents
{
    public class ChatCounterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatCounterViewComponent(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int chatCount = await GetChatCount();
            return View(chatCount);
        }

        private async Task<int> GetChatCount()
        {
            string userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            int chatCount = await _dbContext.ChatMessages
            .Where(u => u.ReceiverId == userId && u.IsRead == false)
            .Select(u => u.SenderId)
            .Distinct()
            .CountAsync();

            return chatCount;
        }
    }
}
