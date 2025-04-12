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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);



            return View();
        }
        public async Task<IActionResult> Chats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            return View();
        }
    }
}
