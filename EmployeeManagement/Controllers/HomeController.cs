using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
	//[Route("[controller]/[action]")]
    
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository empRepo;

		public IHostingEnvironment HostingEnv;
        private readonly ILogger<HomeController> logger;
        private readonly IDataProtector protector;


		public HomeController(IEmployeeRepository _empRepo, IHostingEnvironment hostingEnv,
                              ILogger<HomeController> logger, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            empRepo = _empRepo;
			HostingEnv = hostingEnv;
            this.logger = logger;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

		//[Route("{id?}")]
        [AllowAnonymous]
		public ViewResult Details(string id)
        {
            //throw new Exception("/Erorrr!!!");

            int empID = Convert.ToInt32(protector.Unprotect(id));

			Employee employee = empRepo.getEmployee(empID);
			if(employee == null)
			{
				Response.StatusCode = 404;
				return View("EmployeeNotFound", empID);

			}

			HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
			{
				Employee = empRepo.getEmployee(empID),
			    PageTitle = "PageTitle"
			};

            return View(homeDetailsViewModel);
        }

		//[Route("~/Home")]
		//[Route("~/")]
        [AllowAnonymous]
		public ViewResult Index()
		{
            IEnumerable<Employee> model = empRepo.getAllEmployees().Select(e =>
            {
                e.EncryptedId = protector.Protect(e.ID.ToString());
                return e;
            });

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

        [AcceptVerbs("Get","Delete")]
        [Authorize]
        public IActionResult Delete(int ID)
        {
            if (ModelState.IsValid)
            {
                Employee emp = empRepo.Delete(ID);
                return RedirectToAction("Index");
            }

            return View("Index");
        }
	}

}
