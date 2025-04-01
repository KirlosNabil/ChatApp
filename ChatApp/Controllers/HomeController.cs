using System.Diagnostics;
using System.Security.Claims;
using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet]
        public async Task<IActionResult> Search(string username)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<UserViewModel> users = await _homeService.SearchUser(userId, username);

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
