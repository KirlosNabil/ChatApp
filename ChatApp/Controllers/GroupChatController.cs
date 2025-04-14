using System.Security.Claims;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class GroupChatController : Controller
    {
        private readonly IGroupChatService _groupChatService;
        public GroupChatController(IGroupChatService groupChatService)
        {
            _groupChatService = groupChatService;
        }
        public async Task<IActionResult> GroupChat(int groupId)
        {
            GroupChatViewModel groupChatViewModel = await _groupChatService.GetGroupChat(groupId);

            return View(groupChatViewModel);
        }
        public async Task<IActionResult> GroupChats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<GroupChatsViewModel> groupChats = await _groupChatService.GetGroupChats(userId);

            return View(groupChats);
        }
    }
}
