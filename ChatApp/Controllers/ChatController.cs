using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Chat(string friendId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ChatViewModel chat = await _chatService.GetChat(userId, friendId);

            return View(chat);
        }
        public async Task<IActionResult> Chats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ChatsViewModel> chats = await _chatService.GetChats(userId);

            return View(chats);
        }
    }
}
