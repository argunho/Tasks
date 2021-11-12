using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tasks.Models;
using Tasks.Repositories;
using TasksUI.Models;
using TasksUI.WebApiConnect;

namespace TasksUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
        private readonly SignInManager<Users> _signInManager;

        private ApiConnect _api = new ApiConnect();

        public HomeController(ILogger<HomeController> logger, SignInManager<Users> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        [Route("")]
        public IActionResult Index()
        {
            if (!HelpRepository.loc.Exists("hashToken") && User.Identity.IsAuthenticated)
            {
                _signInManager.SignOutAsync();
                return base.Redirect("/");
            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
