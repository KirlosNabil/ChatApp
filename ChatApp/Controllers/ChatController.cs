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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User friend = _dbContext.Users.FirstOrDefault(x => x.Id == friendId);

            ChatViewModel chat = new ChatViewModel();

            List<ChatMessage> chatMessages = _dbContext.ChatMessages
                    .Where(m => ((m.SenderId == userId && m.ReceiverId == friendId) ||
                                (m.SenderId == friendId && m.ReceiverId == userId)))
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
                if(ms.ReceiverId == userId) 
                {
                    ms.IsRead = true;
                }
                _dbContext.ChatMessages.Update(ms);
            }
            _dbContext.SaveChanges();
            chat.friendId = friendId;
            chat.friendName = friend.FirstName + " " + friend.LastName;
            return View(chat);
        }
        public IActionResult Chats()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ChatMessage> messages = _dbContext.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => g.OrderByDescending(m => m.Date).FirstOrDefault())
            .ToList();

            List<ChatsViewModel> chats = new List<ChatsViewModel>();
            foreach(ChatMessage c in messages)
            {
                ChatsViewModel cvm = new ChatsViewModel();
                User friend;
                if (c.SenderId == userId)
                {
                    friend = _dbContext.Users.FirstOrDefault(u => u.Id == c.ReceiverId);
                    cvm.lastMesasageSenderName = "You";
                }
                else 
                {
                    friend = _dbContext.Users.FirstOrDefault(u => u.Id == c.SenderId);
                    cvm.lastMesasageSenderName = friend.FirstName + " " + friend.LastName;
                }
                cvm.lastMessage = c.Message;
                cvm.friendName = friend.FirstName + " " + friend.LastName;
                cvm.friendId = friend.Id;
                cvm.countUnreadMessages = _dbContext.ChatMessages.Count(m=> m.IsRead == false && (m.ReceiverId == userId));
                chats.Add(cvm);
            }
            return View(chats);
        }
    }
}
