using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MoonBucksCafe.Models;
using System.Diagnostics;

namespace MoonBucksCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string? connectionString;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

            // Retrieve the connection string manually
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            // Clear TempData (if you want to remove any temporary data set in the session)
            TempData.Clear();

            // Sign out from the authentication scheme (cookie-based authentication in this case)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session (optional, if you want to remove all session data)
            HttpContext.Session.Clear();

            // Return the view (or redirect if needed)
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            TempData.Clear(); // Clear session or token
                              // Sign out from the authentication scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session (optional)
            HttpContext.Session.Clear();

            // Redirect to login page or home
            return RedirectToAction("Login", "Order");
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

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult Reservation()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveContact(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                // Insert data into the database using ADO.NET
                string query = "INSERT INTO ContactForms (Name, Email, Subject, Message, SubmissionDate) " +
                               "VALUES (@Name, @Email, @Subject, @Message, @SubmissionDate)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Subject", model.Subject);
                    cmd.Parameters.AddWithValue("@Message", model.Message);
                    cmd.Parameters.AddWithValue("@SubmissionDate", DateTime.Now); // Current date and time

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery(); // Execute the query
                        ViewBag.Message = "Your message has been successfully submitted!";
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors
                        ViewBag.Message = "Error occurred: " + ex.Message;
                    }
                }
            }
            return View("Contact");
        }

    }
}
