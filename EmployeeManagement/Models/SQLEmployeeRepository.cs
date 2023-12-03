using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
	public class SQLEmployeeRepository : IEmployeeRepository
	{
		private readonly AppDbContext context;

		public SQLEmployeeRepository(AppDbContext context)
		{
			this.context = context;
		}

		public Employee Add(Employee employee)
		{
			context.Employees.Add(employee);
			context.SaveChanges();
			return employee;
		}

		public Employee Delete(int id)
		{
			Employee emp = context.Employees.Find(id);
			if(emp != null)
			{
				context.Employees.Remove(emp);
				context.SaveChanges();
			}

			return emp;
		}

		public Employee Update(Employee employee)
		{
			var emp = context.Employees.Attach(employee);
			emp.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			context.SaveChanges();
			return employee;

		}

		public IEnumerable<Employee> getAllEmployees()
		{
			return context.Employees;
		}

		public Employee getEmployee(int id)
		{
			return context.Employees.Find(id);
		}
	}
}
