using System.Diagnostics;
using System.Security.Claims;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Search(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return View("Index", new List<User>());
            }

            var names = username.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var users = new List<User>();
            if (names.Length == 1)
            {
                users  = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])).ToList();
            }
            else if (names.Length >= 2)
            {
                users = _dbContext.Users.Where(u => u.FirstName.Contains(names[0]) && u.LastName.Contains(names[1])).ToList();
            }
            return View("Index", users);
        }

        public IActionResult SentRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            List<string> sentRequests = new List<string>();
            foreach(string name in user.FriendList)
            {
                sentRequests.Add(name);
            }
            return View(sentRequests);

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
