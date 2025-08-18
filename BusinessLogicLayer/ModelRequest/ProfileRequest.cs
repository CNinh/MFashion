using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelRequest
{
    public class ProfileRequest
    {
    }

    public class UpdateProfileRequest
    {
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

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must contain 10 digits and start with 0")]
        [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Slug is required")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug can only contain lowercase letters and numbers")]
        [DefaultValue("johnweak")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Range(typeof(DateTime), "1920-01-01", "2012-12-31", ErrorMessage = "You must be at least 12 years old")]
        [DefaultValue("2000-01-01")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public bool Gender { get; set; }
    }

}
