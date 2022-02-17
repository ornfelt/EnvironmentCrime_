using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentCrime.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vänligen fyll i användarnamn")]
        [Display(Name = "Användarnamn:")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i lösenord")]
        [Display(Name = "Lösenord:")]
        [UIHint("password")] //hides password characters on screen
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
