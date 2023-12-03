using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> empList;

        public MockEmployeeRepository()
        {
            empList = new List<Employee>()
            {
                new Employee() {ID = 1 , Name = "Mona" , Email = "mona@gmail.com" , Department = Dept.HR},
                new Employee() {ID = 2 , Name = "Ali", Email = "Ali@gmail.com" , Department = Dept.IT},
                new Employee() {ID = 3 , Name = "Nora", Email = "Nora@gmail.com" , Department = Dept.Payroll}
            };
        }

		public Employee Add(Employee employee)
		{
			employee.ID = empList.Max(e => e.ID) + 1;
			empList.Add(employee);

			return employee;
		}

		public Employee Delete(int id)
		{
			Employee employee = empList.FirstOrDefault(e => e.ID == id);
			if(employee != null)
			{
				empList.Remove(employee);
			}
			return employee;
		}

		public Employee Update(Employee employee)
		{
			Employee emp = empList.FirstOrDefault(e => e.ID == employee.ID);
			if(emp != null)
			{
				emp.Name = employee.Name;
				emp.Email = employee.Email;
				emp.Department = employee.Department;
			}

			return employee;
		}

		public IEnumerable<Employee> getAllEmployees()
		{
			return empList;
		}

		public Employee getEmployee(int id)
        {
            return empList.FirstOrDefault(e => e.ID == id);
        }
    }
}
