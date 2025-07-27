using Microsoft.AspNetCore.Mvc;
using EventRegistration.Models;
using System.Diagnostics;

namespace EventRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var eventInfo = new EventInfo
            {
                Name = _configuration["EventDetails:Name"] ?? "Ideaspark",
                Date = _configuration["EventDetails:Date"] ?? "Coming Soon",
                Location = _configuration["EventDetails:Location"] ?? "DEI BEL BLOCK",
                Description = _configuration["EventDetails:Description"] ?? "An exciting tech event",
                Features = _configuration.GetSection("EventDetails:Features").Get<List<string>>() ?? new List<string>()
            };

            return View(eventInfo);
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

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}