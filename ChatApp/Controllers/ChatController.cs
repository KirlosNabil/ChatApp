using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ChatController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string userId)
    {
        var currentUserId = _userManager.GetUserId(User);
        var messages = await _context.ChatMessages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                        (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.Date)
            .ToListAsync();

        ViewBag.ReceiverId = userId;
        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string receiverId, string message)
    {
        var senderId = _userManager.GetUserId(User);
        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId = receiverId });
    }
}
