using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<User> _userManager;

        public ChatController(UserManager<User> userManager)
        {
            _userManager = userManager;
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

            var messages = new List<ChatMessage>();
            return View(messages);
        }
    }
}
