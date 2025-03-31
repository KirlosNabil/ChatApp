using ChatApp.Models;
using ChatApp.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;
using ChatApp.Services;

namespace ChatApp.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;
        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }
        public async Task<IActionResult> FriendRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendRequestViewModel> friendRequestViewModels = await _friendService.GetUserFriendRequests(userId);
            
            return View(friendRequestViewModels);
        }
        public async Task<IActionResult> SentRequests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<SentRequestViewModel> sentRequestViewModels = await _friendService.GetUserSentRequests(userId);
            
            return View(sentRequestViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRequest(string receiverId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _friendService.RemoveRequest(userId, receiverId);

            return RedirectToAction("SentRequests");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _friendService.RemoveFriend(userId, friendId);

            return RedirectToAction("Friends");
        }
        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string senderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _friendService.AcceptFriendRequest(userId, senderId);

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(string senderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _friendService.RejectFriendRequest(userId, senderId);
            
            return Ok();
        }
        public async Task<IActionResult> Friends()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<FriendViewModel> friends = await _friendService.GetUserFriends(userId);
            
            return View(friends);
        }
    }
}
