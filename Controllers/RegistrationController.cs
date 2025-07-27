using Microsoft.AspNetCore.Mvc;
using EventRegistration.Models;
using System.Data.SqlClient;
using System.Data;

namespace EventRegistration.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IConfiguration configuration, ILogger<RegistrationController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new Registration());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration registration)
        {
            if (!ModelState.IsValid)
            {
                return View(registration);
            }

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);

                var query = @"INSERT INTO Registrations (Name, Email, PhoneNumber, College, Department, Year, RegistrationDate) 
                             VALUES (@Name, @Email, @PhoneNumber, @College, @Department, @Year, @RegistrationDate)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", registration.Name);
                command.Parameters.AddWithValue("@Email", registration.Email);
                command.Parameters.AddWithValue("@PhoneNumber", registration.PhoneNumber);
                command.Parameters.AddWithValue("@College", registration.College);
                command.Parameters.AddWithValue("@Department", registration.Department);
                command.Parameters.AddWithValue("@Year", registration.Year);
                command.Parameters.AddWithValue("@RegistrationDate", registration.RegistrationDate);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation($"Registration successful for {registration.Email}");

                return RedirectToAction("Confirmation", new { name = registration.Name, email = registration.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(registration);
            }
        }

        public IActionResult Confirmation(string name, string email)
        {
            ViewBag.Name = name;
            ViewBag.Email = email;
            return View();
        }
    }
}