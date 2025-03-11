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
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult StartChat(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var friend = _dbContext.Users.FirstOrDefault(x => x.Id == friendId);
            
            ChatViewModel chat = new ChatViewModel();

            List<ChatMessage> chatMessages = _dbContext.ChatMessages
                    .Where(m => ((m.SenderId == userId && m.ReceiverId == friendId) ||
                                (m.SenderId == friendId && m.ReceiverId == userId)) && m.Delivered == true)
                    .OrderBy(m => m.Date)
                    .ToList();

            foreach(ChatMessage ms in chatMessages) 
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
            return View("~/Views/Chat/Index.cshtml", chat);
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
            && fr.Status == FriendRequestStatus.Pending || fr.Status == FriendRequestStatus.Sent)
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

        public IActionResult FriendRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendRequest> friendRequests= _dbContext.FriendRequests
                .Where(u => u.ReceiverId == userId && u.Status == FriendRequestStatus.Pending).ToList();

            List<FriendRequestViewModel> frvm = new List<FriendRequestViewModel>();
            foreach(FriendRequest f in friendRequests)
            {
                string senderId = f.SenderId;
                User sender = _dbContext.Users.FirstOrDefault(u => u.Id == senderId);
                string firstName = sender.FirstName;
                string lastName = sender.LastName;
                FriendRequestViewModel fvmm = new FriendRequestViewModel()
                {
                    SenderFirstName = firstName,
                    SenderLastName = lastName,
                    SenderId = senderId,
                };
                frvm.Add(fvmm);
            }
            return View(frvm);
        }

        public IActionResult SentRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendRequest> friendRequests = _dbContext.FriendRequests
                .Where(u => u.SenderId == userId && (u.Status == FriendRequestStatus.Pending || u.Status == FriendRequestStatus.Sent)).ToList();

            List<SentRequestViewModel> frvm = new List<SentRequestViewModel>();
            foreach (FriendRequest f in friendRequests)
            {
                string receiverId = f.ReceiverId;
                User receiver = _dbContext.Users.FirstOrDefault(u => u.Id == receiverId);
                string firstName = receiver.FirstName;
                string lastName = receiver.LastName;
                SentRequestViewModel srvm = new SentRequestViewModel()
                {
                    ReceiverFirstName = firstName,
                    ReceiverLastName = lastName,
                    ReceiverId = receiverId
                };
                frvm.Add(srvm);
            }
            return View(frvm);
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(string id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            User sender = _dbContext.Users.FirstOrDefault(x => x.Id == id);

            user.FriendList.Add(sender.Id);
            sender.FriendList.Add(user.Id);

            FriendRequest friendRequest = _dbContext.FriendRequests
                .Where(f => f.SenderId == id && f.ReceiverId == userId)
                .FirstOrDefault();
            friendRequest.Status = FriendRequestStatus.Accepted;
            _dbContext.FriendRequests.Update(friendRequest);
            _dbContext.Users.Update(user);
            _dbContext.Users.Update(sender);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult RejectFriendRequest(string id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            FriendRequest friendRequest = _dbContext.FriendRequests
                .Where(f => f.SenderId == id && f.ReceiverId == userId)
                .FirstOrDefault();
            friendRequest.Status = FriendRequestStatus.Rejected;
            _dbContext.FriendRequests.Update(friendRequest);
            _dbContext.SaveChanges();
            return Ok();
        }

        public IActionResult Friends()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            List<FriendViewModel> friends = new List<FriendViewModel>();
            foreach(string friend in user.FriendList)
            {
                User f = _dbContext.Users.FirstOrDefault(u => u.Id == friend);
                FriendViewModel fvm = new FriendViewModel()
                {
                    FriendId = friend,
                    FirstName = f.FirstName,
                    LastName = f.LastName
                };
                friends.Add(fvm);
            }
            return View(friends);
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
