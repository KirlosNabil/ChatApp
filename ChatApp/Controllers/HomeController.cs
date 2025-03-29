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
            var users = new List<UserViewModel>();
            var requestsUserIds = _dbContext.FriendRequests
            .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId)
                        && (fr.Status != FriendRequestStatus.Rejected))
            .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
            .ToHashSet();
            List<User> usersWithRelation = new List<User>();
            List<User> usersWithNoRelation = new List<User>();
            if (names.Length == 1)
            {
                usersWithRelation = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.Id != userId
                && requestsUserIds.Contains(u.Id)).ToList();

                usersWithNoRelation = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.Id != userId
                && !requestsUserIds.Contains(u.Id)).ToList();
            }
            else if (names.Length >= 2)
            {
                usersWithRelation = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.LastName.Contains(names[1])
                && u.Id != userId
                && !requestsUserIds.Contains(u.Id)).ToList();

                usersWithNoRelation = _dbContext.Users.Where(u => u.FirstName.Contains(names[0])
                && u.LastName.Contains(names[1])
                && u.Id != userId
                && !requestsUserIds.Contains(u.Id)).ToList();
            }
            foreach (var user in usersWithRelation)
            {
                UserViewModel retUser = new UserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
                var friendRequest = _dbContext.FriendRequests.FirstOrDefault(u => (u.SenderId == userId && u.ReceiverId == user.Id)
                || (u.SenderId == user.Id && u.ReceiverId == userId));
                if (friendRequest.Status == FriendRequestStatus.Accepted)
                {
                    retUser.Relation = UserRelation.Friend;
                }
                else
                {
                    if (friendRequest.SenderId == userId)
                    {
                        retUser.Relation = UserRelation.SentFriendRequest;
                    }
                    else
                    {
                        retUser.Relation = UserRelation.ReceivedFriendRequest;
                    }
                }
                users.Add(retUser);
            }
            foreach(var user in usersWithNoRelation)
            {
                UserViewModel retUser = new UserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Relation = UserRelation.NoRelation
                };
                users.Add(retUser);
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
