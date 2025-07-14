using System.ComponentModel.DataAnnotations;

namespace MoonBucksCafe.Models
{
    
    public class Register
    {
        [Required(ErrorMessage = "User Name is required")]
        [StringLength(100, ErrorMessage = "User Name cannot be longer than 100 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid Mobile Number")]
        public string Mobile { get; set; }
    }
    public class OrderDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string Paymode { get; set; }
        public bool IsDeliver { get; set; }

        public string UserEmail { get; set; }
    }
    public class TableBooking
    {
        public int Id { get; set; }  // This will be auto-incremented in the database.
        public string Name { get; set; }
        public string Email { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public int PersonCount { get; set; }
        public string BookingStatus { get; set; }
    }

    public class ContactForm
    {
        public int Id { get; set; } // Auto-incremented in the database
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SubmissionDate { get; set; } // To store when the form was submitted
    }
}
