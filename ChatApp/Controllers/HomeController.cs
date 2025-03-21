using System.Diagnostics;
using System.Security.Claims;
using ChatApp.Data;
using ChatApp.Models;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public HomeController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Search(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return View("Index", new List<User>());
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var names = username.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var users = new List<User>();
            var sentRequestUserIds = _dbContext.FriendRequests
            .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId)
            && fr.Status == FriendRequestStatus.Pending || fr.Status == FriendRequestStatus.Sent || fr.Status == FriendRequestStatus.Accepted)
            .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
            .ToHashSet();
            if (names.Length == 1)
            {
                users  = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.Id != userId
                && !sentRequestUserIds.Contains(u.Id)).ToList();
            }
            else if (names.Length >= 2)
            {
                users = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.LastName.Contains(names[1])
                && u.Id != userId
                && !sentRequestUserIds.Contains(u.Id)).ToList();
            }
            return View("Index", users);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
