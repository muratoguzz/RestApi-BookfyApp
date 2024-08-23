using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record UserForRegistrationDto
    {
        public String? FirstName { get; init; }
        public String? LastName { get; init; }

        [Required(ErrorMessage = "Username is required")]
        public String? UserName { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public String? Password { get; init; }

        public String? Email { get; init; }
        public String? PhoneNumber { get; init; }

        public ICollection<string>? Roles { get; init; }


    }
}
