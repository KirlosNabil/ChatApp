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

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.UserId = user.Id;
        return View();
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
