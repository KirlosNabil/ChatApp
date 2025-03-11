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
        private readonly ApplicationDbContext _dbContext;
        public ChatController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IActionResult Chat(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var friend = _dbContext.Users.FirstOrDefault(x => x.Id == friendId);

            ChatViewModel chat = new ChatViewModel();

            List<ChatMessage> chatMessages = _dbContext.ChatMessages
                    .Where(m => ((m.SenderId == userId && m.ReceiverId == friendId) ||
                                (m.SenderId == friendId && m.ReceiverId == userId)) && m.Delivered == true)
                    .OrderBy(m => m.Date)
                    .ToList();

            foreach (ChatMessage ms in chatMessages)
            {
                User sender = _dbContext.Users.FirstOrDefault(u => u.Id == ms.SenderId);
                string name = "";
                if (ms.SenderId == userId)
                {
                    name = "You";
                }
                else
                {
                    name = sender.FirstName + " " + sender.LastName;
                }

                chat.ChatMessages.Add(new Tuple<ChatMessage, string>(ms, name));
            }
            chat.friendId = friendId;
            chat.friendName = friend.FirstName + " " + friend.LastName;
            return View(chat);
        }
    }
}
