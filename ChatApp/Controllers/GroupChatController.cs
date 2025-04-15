using System.Security.Claims;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class GroupChatController : Controller
    {
        private readonly IGroupChatService _groupChatService;
        private readonly IFriendService _friendService;
        public GroupChatController(IGroupChatService groupChatService, IFriendService friendService)
        {
            _groupChatService = groupChatService;
            _friendService = friendService;
        }
        public async Task<IActionResult> GroupChat(int groupId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            GroupChatViewModel groupChatViewModel = await _groupChatService.GetGroupChat(userId, groupId);

            return View(groupChatViewModel);
        }
        public async Task<IActionResult> GroupChats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<GroupChatsViewModel> groupChats = await _groupChatService.GetGroupChats(userId);

            return View(groupChats);
        }
        public async Task<IActionResult> CreateGroupChat()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendViewModel> friends = await _friendService.GetUserFriends(userId);
            CreateGroupChatViewModel model = new CreateGroupChatViewModel
            {
                Friends = friends
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroupChat(CreateGroupChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _groupChatService.CreateGroupChat(userId, model);

                return RedirectToAction("GroupChats", "GroupChat");
            }
            return View(model);
        }
    }
}
