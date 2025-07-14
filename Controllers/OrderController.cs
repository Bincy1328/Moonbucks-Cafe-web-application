using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MoonBucksCafe.Models;

namespace MoonBucksCafe.Controllers
{
    public class OrderController : Controller
    {
        string? connectionString;
        public OrderController()
        {
            var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

            // Retrieve the connection string manually
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult Login()
        {
            return View();
        }

       
        public IActionResult Cart()
        {
            string userEmail = string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ? "" : HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // If no session, redirect to login or show an error message
                return RedirectToAction("Login", "Order");
            }
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }
        
        public IActionResult Payment()
        {
            string userEmail = string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ? "" : HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // If no session, redirect to login or show an error message
                return RedirectToAction("Login", "Order");
            }
            return View();
        }

        public IActionResult BookingDetails()
        {
            string userEmail = string.IsNullOrEmpty(HttpContext.Session.GetString("BookingEmail")) ? "" : HttpContext.Session.GetString("BookingEmail");
            var bookingDetails = GetAllBookings(userEmail);
            return View(bookingDetails);
        }
        public ActionResult InsertUser(string userName, string email, string password, string mobile)
        {

            // SQL query to insert a new user
            string query = "INSERT INTO Users (UserName, Email, Password, Mobile, InsertedOn) VALUES (@UserName, @Email, @Password, @Mobile, @InsertedOn)";

            // Create a connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a command object to execute the query
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command to avoid SQL injection
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Mobile", mobile);
                    command.Parameters.AddWithValue("@InsertedOn", DateTime.Now); // Set the current time for InsertedOn

                    // Execute the command to insert the data
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if any rows were affected
                    return RedirectToAction("Login"); // Return true if the insert was successful
                }
            }

        }
        [HttpPost]
        public JsonResult Login(string Email, string Password)
        {
            // The result to send back to the AJAX call
            var result = new { success = false, message = "Invalid credentials.", url = "" };



            // Use a parameterized query to prevent SQL injection
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // SQL query to check if the user exists with the provided credentials
                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    // Execute the query and get the result
                    int userExists = (int)cmd.ExecuteScalar();

                    if (userExists > 0)
                    {
                        HttpContext.Session.SetString("UserEmail", Email);
                        if (Email == "moonbucks@gmail.com")
                        {
                            result = new { success = true, message = "Login successful.", url = "/Order/AdminOrderDetails" };
                        }
                        else
                        {
                            result = new { success = true, message = "Login successful.", url = "/Order/Cart" };
                        }
                        
                        HttpContext.Session.SetString("LoginUserEmail", Email);
                    }
                    else
                    {
                        result = new { success = false, message = "Invalid email or password.", url = "" };
                    }
                }
            }


            // Return the result as JSON
            return Json(result);
        }

        
        [HttpPost]
        public ActionResult PlaceOrder([FromBody] List<OrderDetails> objOrder)
        {
            string userEmail = string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ? "kowsi@gmail.com" : HttpContext.Session.GetString("UserEmail");
            if (objOrder == null || objOrder.Count == 0)
            {
                return Json(new { success = false, message = "No order data received." });
            }

            string query = "INSERT INTO OrderDetails (Name, Price, Quantity, OrderDate, Paymode, IsDeliver,UserEmail) " +
                           "VALUES (@Name, @Price, @Quantity, @OrderDate, @Paymode, @IsDeliver,@UserEmail)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var order in objOrder)
                    {
                        if (order != null)
                        {
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Name", order.Name);
                                command.Parameters.AddWithValue("@Price", order.Price);
                                command.Parameters.AddWithValue("@Quantity", order.Quantity);
                                command.Parameters.AddWithValue("@OrderDate", DateTime.Now); // Current date for the order
                                command.Parameters.AddWithValue("@Paymode", "CashandDelivery"); // Assuming this is coming from the client
                                command.Parameters.AddWithValue("@IsDeliver", order.IsDeliver); // Assuming initial status is false
                                command.Parameters.AddWithValue("@UserEmail", userEmail);
                                // Execute the command to insert data into the database
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return Json(new { success = true, message = "Orders placed successfully!" });
            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new { success = false, message = "An error occurred while placing the order: " + ex.Message });
            }
        }

        
        public ActionResult OrderDetails()
        {
            string userEmail = string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ? "" : HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // If no session, redirect to login or show an error message
                return RedirectToAction("Login", "Order");
            }            
            
            List<OrderDetails> orderDetails = GetOrderDetails(userEmail);
            List<TableBooking> bookingDetails = GetAllBookings(userEmail);
            var model = new Tuple<List<OrderDetails>, List<TableBooking>>(orderDetails, bookingDetails);
            return View(model);
        }
        public ActionResult AdminOrderDetails()
        {           
            List<OrderDetails> orderDetails = GetOrderDetails("");
            List<TableBooking> bookingDetails = GetAllBookings("");
            List<ContactForm> contactDetails = GetContactDetails();
            var model = new Tuple<List<OrderDetails>, List<TableBooking>, List<ContactForm>>(orderDetails, bookingDetails, contactDetails); ;
            return View(model);
        }

        public List<OrderDetails> GetOrderDetails(string UserEmail)
        {
            List<OrderDetails> orders = new List<OrderDetails>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"select * from OrderDetails where UserEmail=@UserEmail or isnull(@UserEmail,'')=''";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new OrderDetails
                    {
                        Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0,
                        Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                        OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : DateTime.MinValue,
                        Paymode = reader["Paymode"] != DBNull.Value ? reader["Paymode"].ToString() : "COD",
                        IsDeliver = reader["IsDeliver"] != DBNull.Value && Convert.ToBoolean(reader["IsDeliver"]),
                        UserEmail = reader["UserEmail"] != DBNull.Value ? reader["UserEmail"].ToString() : string.Empty
                    });
                }
            }
            return orders;
        }

        [HttpPost]
        public ActionResult MarkAsDelivered(int id)
        {
            // Call the method to update the delivery status using ADO.NET
            UpdateDeliveryStatus(id);

            // Redirect back to the Details page to reflect the changes
            return RedirectToAction("AdminOrderDetails");
        }
        private void UpdateDeliveryStatus(int orderId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE OrderDetails SET IsDeliver = @IsDeliver WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    // Use parameters to prevent SQL injection
                    command.Parameters.Add("@IsDeliver", SqlDbType.Bit).Value = true; // Set IsDeliver to true
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = orderId; // Specify the order ID

                    connection.Open();
                    command.ExecuteNonQuery(); // Execute the query
                }
            }

        }

        [HttpPost]
        public ActionResult TableBook(string name, string email, string bookingDate, string bookingTime, int personCount)
        {
            try
            {
                // Save the booking to the database
                SaveBooking(name, email, bookingDate, bookingTime, personCount);
                HttpContext.Session.SetString("BookingEmail", email);
                // Redirect to a confirmation page or back to the booking form with a success message
                ViewBag.Message = "Your table has been booked successfully!";
                return RedirectToAction("BookingDetails");
            }
            catch (Exception ex)
            {
                // Log the error and show a friendly message
                ViewBag.Message = "There was an error while booking your table. Please try again.";
                return View();
            }
        }

        // Method to save the booking details into the database using ADO.NET
        public void SaveBooking(string name, string email, string bookingDate, string bookingTime, int personCount)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO TableBookings (Name, Email, BookingDate, BookingTime, PersonCount,BookingStatus) " +
                               "VALUES (@Name, @Email, @BookingDate, @BookingTime, @PersonCount,'Confirmed')";

                using (var command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    command.Parameters.Add("@BookingDate", SqlDbType.NVarChar,100).Value = bookingDate;
                    command.Parameters.Add("@BookingTime", SqlDbType.NVarChar,100).Value = bookingTime;
                    command.Parameters.Add("@PersonCount", SqlDbType.Int).Value = personCount;

                    connection.Open();
                    command.ExecuteNonQuery();  // Execute the query (insert the data into the database)
                }
            }
        }        

        // Method to get all bookings from the database
        public List<TableBooking> GetAllBookings(string userEmail)
        {
            var bookings = new List<TableBooking>();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Email, BookingDate, BookingTime, PersonCount,BookingStatus FROM TableBookings where Email=@UserEmail or isnull(@UserEmail,'')=''";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@UserEmail", userEmail);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookings.Add(new TableBooking
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                BookingDate = reader.GetString(3),
                                BookingTime = reader.GetString(4),
                                PersonCount = reader.GetInt32(5),
                                BookingStatus = reader.GetString(6),
                            });
                        }
                    }
                }
            }

            return bookings;
        }

        [HttpPost]
        public ActionResult CancelBooking(int id)
        {
            // Define the SQL query to update the booking status
            string query = "UPDATE TableBookings SET BookingStatus = @BookingStatus WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the command object
                SqlCommand cmd = new SqlCommand(query, connection);

                // Add parameters to avoid SQL injection
                cmd.Parameters.AddWithValue("@BookingStatus", "Canceled");
                cmd.Parameters.AddWithValue("@Id", id);

                try
                {
                    // Open the connection
                    connection.Open();

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Success - Update is done
                        ViewBag.Message = "Booking has been successfully canceled.";
                    }
                    else
                    {
                        // If no rows were affected, it means the booking ID wasn't found
                        ViewBag.Message = "Booking not found.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
            }

            // Redirect back to the index page or where you'd like to show updated details
            return RedirectToAction("OrderDetails");
        }

        public List<ContactForm> GetContactDetails()
        {
            List<ContactForm> contactDetails = new List<ContactForm>();

            string query = "SELECT Id, Name, Email, Subject, Message, SubmissionDate FROM ContactForms";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContactForm contact = new ContactForm
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Message = reader["Message"].ToString(),
                            SubmissionDate = Convert.ToDateTime(reader["SubmissionDate"])
                        };
                        contactDetails.Add(contact);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors (e.g., logging)
                    ViewBag.Error = "An error occurred: " + ex.Message;
                }
            }

            return contactDetails;
        }

    }
}

