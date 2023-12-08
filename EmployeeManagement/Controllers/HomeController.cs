using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
	//[Route("[controller]/[action]")]
    
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository empRepo;

		public IHostingEnvironment HostingEnv;

		public HomeController(IEmployeeRepository _empRepo, IHostingEnvironment hostingEnv)
        {
            empRepo = _empRepo;
			HostingEnv = hostingEnv;
		}

		//[Route("{id?}")]
        [AllowAnonymous]
		public ViewResult Details(int? id)
        {
            //throw new Exception("/Erorrr!!!");

			Employee employee = empRepo.getEmployee(id.Value);
			if(employee == null)
			{
				Response.StatusCode = 404;
				return View("EmployeeNotFound", id.Value);

			}
			HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
			{
				Employee = empRepo.getEmployee(id??1),
			    PageTitle = "PageTitle"
			};

            return View(homeDetailsViewModel);
        }

		//[Route("~/Home")]
		//[Route("~/")]
        [AllowAnonymous]
		public ViewResult Index()
		{
			IEnumerable<Employee> model = empRepo.getAllEmployees();
			return View(model);
		}

		[HttpGet]
		public ViewResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(EmployeeCreateViewModel model)
		{
			
			if (ModelState.IsValid)
			{
				string uniqueFileName = ProcessUploadedFile(model);
				Employee newEmp = new Employee
				{
					Name = model.Name,
					Email = model.Email,
					Department = model.Department,
					PhotoPath = uniqueFileName
				};
				empRepo.Add(newEmp);
				return RedirectToAction("details", new { id = newEmp.ID });
			}

			return View();
		}

		private string ProcessUploadedFile(EmployeeCreateViewModel model)
		{
			string uniqueFileName = null;
			if (model.Photos != null && model.Photos.Count > 0)
			{
				foreach (IFormFile photo in model.Photos)
				{
					string uploadsFolder = Path.Combine(HostingEnv.WebRootPath, "images");
					uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						photo.CopyTo(fileStream);
					}
					
				}
			}

			return uniqueFileName;
		}

		[HttpGet]
        [Authorize]
		public ViewResult Edit(int id)
		{
			Employee employee = empRepo.getEmployee(id);
			
			EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
			{
				ID = employee.ID,
				Name = employee.Name,
				Email = employee.Email,
				Department = employee.Department,
				ExistingPhotoPath = employee.PhotoPath
			};

			return View(employeeEditViewModel);
		}

		[HttpPost]
        [Authorize]
		public IActionResult Edit(EmployeeEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				Employee employee = empRepo.getEmployee(model.ID);
				employee.Name = model.Name;
				employee.Email = model.Email;
				employee.Department = model.Department;

				if (model.Photos != null)
				{
					if (model.ExistingPhotoPath != null)
					{
						string filePath = Path.Combine(HostingEnv.WebRootPath, "images", model.ExistingPhotoPath);
						System.IO.File.Delete(filePath);
					}
					employee.PhotoPath = ProcessUploadedFile(model);
				}

				Employee updatedEmp = empRepo.Update(employee);
				return RedirectToAction("Index");
			}
			return View(model);
		}

	}

}
