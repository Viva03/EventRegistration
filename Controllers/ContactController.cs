using Microsoft.AspNetCore.Mvc;
using EventRegistration.Models;
using System.Data.SqlClient;

namespace EventRegistration.Controllers
{
    public class ContactController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IConfiguration configuration, ILogger<ContactController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return View(contact);
            }

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);

                var query = @"INSERT INTO ContactMessages (Name, Email, Message, CreatedDate) 
                             VALUES (@Name, @Email, @Message, @CreatedDate)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", contact.Name);
                command.Parameters.AddWithValue("@Email", contact.Email);
                command.Parameters.AddWithValue("@Message", contact.Message);
                command.Parameters.AddWithValue("@CreatedDate", contact.CreatedDate);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation($"Contact message received from {contact.Email}");

                ViewBag.SuccessMessage = "Thank you for your message! We'll get back to you soon.";
                return View(new Contact()); // Clear the form
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contact message");
                ModelState.AddModelError("", "An error occurred while sending your message. Please try again.");
                return View(contact);
            }
        }
    }
}