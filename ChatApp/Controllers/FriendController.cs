using ChatApp.Models;
using ChatApp.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;

namespace ChatApp.Controllers
{
    public class FriendController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public FriendController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IActionResult FriendRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendRequest> friendRequests = _dbContext.FriendRequests
                .Where(u => u.ReceiverId == userId && u.Status == FriendRequestStatus.Pending).ToList();

            List<FriendRequestViewModel> frvm = new List<FriendRequestViewModel>();
            foreach (FriendRequest f in friendRequests)
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
        public IActionResult RemoveRequest(string receiverId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = _dbContext.FriendRequests
            .Where(f => f.SenderId == userId && f.ReceiverId == receiverId && f.Status == FriendRequestStatus.Pending)
            .ToList();

            _dbContext.FriendRequests.RemoveRange(requests);
            _dbContext.SaveChanges();
            return RedirectToAction("SentRequests");
        }

        [HttpPost]
        public IActionResult RemoveFriend(string friendId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            user.FriendList.Remove(friendId);
            User friend = _dbContext.Users.FirstOrDefault(u => u.Id == friendId);
            friend.FriendList.Remove(userId);
            _dbContext.Users.Update(user);
            _dbContext.Users.Update(friend);
            _dbContext.SaveChanges();
            var requests = _dbContext.FriendRequests
            .Where(f => (f.SenderId == userId && f.ReceiverId == friendId)
              || (f.SenderId == friendId && f.ReceiverId == userId))
            .ToList();

            _dbContext.FriendRequests.RemoveRange(requests);
            _dbContext.SaveChanges();

            return RedirectToAction("Friends");
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
                .Where(f => f.SenderId == id && f.ReceiverId == userId && f.Status == FriendRequestStatus.Pending)
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
                .Where(f => f.SenderId == id && f.ReceiverId == userId && f.Status == FriendRequestStatus.Pending)
                .FirstOrDefault();
            friendRequest.Status = FriendRequestStatus.Rejected;
            _dbContext.FriendRequests.Update(friendRequest);
            _dbContext.SaveChanges();
            return Ok();
        }

        public IActionResult Friends()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            List<FriendViewModel> friends = new List<FriendViewModel>();
            foreach (string friend in user.FriendList)
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
    }
}
