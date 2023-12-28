using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
	public class EmployeeCreateViewModel
	{
		
		[Required]
		[MaxLength(50, ErrorMessage = "Name can't exceed 50 characters")]
		public string Name { get; set; }
		[Required]
		[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Format")]
		[Display(Name = "Office Email")]
		public string Email { get; set; }
		[Required]
		public Dept? Department { get; set; }
		public List<IFormFile> Photos { get; set; }
	}
}
