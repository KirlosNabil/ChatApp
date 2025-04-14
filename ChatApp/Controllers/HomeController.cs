using System.Diagnostics;
using System.Security.Claims;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IUserService _userService;
        public HomeController(IHomeService homeService, IUserService userService)
        {
            _homeService = homeService;
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> Search(string username)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<UserViewModel> users = await _homeService.SearchUser(userId, username);

            return View("Index", users);
        }
        public async Task<IActionResult> UserProfile(string userId)
        {
            string myId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewUserViewModel viewUserViewModel = await _userService.GetViewUserViewModel(myId, userId);

            return View(viewUserViewModel);
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
