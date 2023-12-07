using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        //we can aslo use ValidEmailDomainAttribute but it's not necessary
        //we'll call allowedDomain in the constractor
        [ValidEmailDomain(allowedDomain:"gmail.com",ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage = "Password and Connfirmation Password don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
