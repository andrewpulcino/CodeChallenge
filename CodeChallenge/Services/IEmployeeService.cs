using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        int GetNumberOfReports(String id);
        Compensation GetCompensationByEmployeeId(string employeeId);
        Compensation AddCompensation(Compensation compensation);
        bool EmployeeHasCompensation(string employeeId);
    }
}
