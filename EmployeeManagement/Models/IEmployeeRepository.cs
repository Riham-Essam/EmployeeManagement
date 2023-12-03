using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public interface IEmployeeRepository
    {
        Employee getEmployee(int id);
		IEnumerable<Employee> getAllEmployees();
		Employee Add(Employee employee);
		Employee Update(Employee employee);
		Employee Delete(int id);
    }
}
