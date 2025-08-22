using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelRequest
{
    public class AuthRequest
    {
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First name can only contain letters and spaces")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [DefaultValue("Nae")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Last name can only contain letters and spaces")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [DefaultValue("Ni")]
        public string LastName { get; set; }
    }

    public class RegisterVendorRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First name can only contain letters and spaces")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [DefaultValue("Nae")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Last name can only contain letters and spaces")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [DefaultValue("Ni")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Shop name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Shop name can only contain letters, numbers and spaces")]
        [StringLength(50, ErrorMessage = "Shop name cannot exceed 50 characters")]
        [DefaultValue("NeGaiShop")]
        public string ShopName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must contain 10 digits and start with 0")]
        [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 digits")]
        public string PhoneNumber { get; set; }
    }

    public class SetupPasswordRequest
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        [DefaultValue("123#Example")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(nameof(Password), ErrorMessage = "Confirm password must be same as password")]
        [DefaultValue("123#Example")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        [DefaultValue("123#Example")]
        public string Password { get; set; }
    }

    public class VerifyLoginOTPRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [Length(6, 6, ErrorMessage = "OTP must contains 6 digit characters")]
        [DefaultValue("123456")]
        public string OTP { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP must contain only numbers")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        [DefaultValue("123#Example")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm password must be same as password")]
        [DefaultValue("123#Example")]
        public string ConfirmNewPassword { get; set; }
    }
}
