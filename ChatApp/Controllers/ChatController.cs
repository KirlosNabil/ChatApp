using ChatApp.Data;
using ChatApp.Models;
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
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public ChatController(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            this._userManager = userManager;
            this._dbContext = dbContext;
        }

        public IActionResult EnterEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartChat(string receiverEmail)
        {
            var user = await _userManager.FindByEmailAsync(receiverEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View("EnterEmail");
            }
            return RedirectToAction("Index", new { userId = user.Id });
        }

        public IActionResult Index(string userId)
        {
            ViewBag.ReceiverId = userId;
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ChatMessage> messages = _dbContext.ChatMessages
            .Where(u => (u.SenderId == senderId && u.ReceiverId == userId) ||
                (u.SenderId == userId && u.ReceiverId == senderId))
            .OrderBy(m => m.Date)
            .ToList();
            ChatViewModel cvm = new ChatViewModel();
            cvm.ChatMessages = new List<Tuple<ChatMessage, string>>();
            foreach(ChatMessage message in messages)
            {
                string sender = message.SenderId;
                string name = _dbContext.Users.FirstOrDefault(u => u.Id == sender).FirstName;
                cvm.ChatMessages.Add(Tuple.Create(message, name));
            }
            return View(cvm);
        }
    }
}
