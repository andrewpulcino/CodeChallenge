using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if (employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if (originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        /// <summary>
        /// Traverses the DirectReports collection property of the employee
        /// with the supplied id, counting the total number of direct
        /// and descendant reports.
        /// </summary>
        /// <param name="id">The employee id of the employee to interrogate.</param>
        /// <returns>The number of total reports under the employee withe the provided id.</returns>
        /// 
        // TODO: Confirm whether this method should return zero
        //       or throw an Exception if the supplied employee id
        //       is invalid or cannot be found.
        public int GetNumberOfReports(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // TODO: Clarify intended behavior when no id was supplied.
                // throw new ArgumentException("An employee id must be supplied.");
                return 0;
            }

            // Get the employee associated with the supplied id, explicitly
            // loading the DirectReports navigation property.
            var employee = _employeeRepository.GetById(id, true);

            // If the employee cannot be found then return
            // zero for the total number of reports.
            if (employee == null)
            {
                // TODO: Clarify intended behavior when an employee could not be found.
                // throw new KeyNotFoundException($"An employee with id = {id} could not be found.");
                return 0;
            }

            int count = 0;

            // Create a lambda function to recursively traverse
            // the tree of direct reports for the given employee
            Func<Employee, int> traverseAndCountReports = null;

            traverseAndCountReports = (emp) =>
            {

                // Get the number of reports for each of this
                // employee's direct reports
                foreach (var directReport in emp.DirectReports)
                {
                    // Use recursion to traverse the tree depth first
                    count += traverseAndCountReports(_employeeRepository.GetById(directReport.EmployeeId, true));
                }

                // Add the number of direct reports associated
                // with this employee. This will be the total
                // number of reports (not just direct reports)
                // for the employee.
                count += emp.DirectReports.Count;

                return count;
            };

            // Invoke the lambda to get the count of direct and descendant repots
            count += traverseAndCountReports(employee);


            return count;
        }

        /// <summary>
        /// Traverses the DirectReports collection property of the supplied
        /// employee, counting the total number of direct
        /// and descendant reports.
        /// </summary>
        /// <param name="employee">The employee to interrogate.</param>
        /// <returns>The number of total reports under the supplied employee.</returns>
        /// 
        public int GetNumberOfReports(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentException("The supplied employee is not valid.");
            }

            return GetNumberOfReports(employee.EmployeeId);
        }

        public Compensation GetCompensationByEmployeeId(string employeeId)
        {
            if (String.IsNullOrWhiteSpace(employeeId))
            {
                return null;
            }

            return _compensationRepository.GetByEmployeeId(employeeId);
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            if (compensation == null)
            {
                return null;
            }

            _compensationRepository.Add(compensation);

            _compensationRepository.SaveAsync().Wait();

            return compensation;

        }

        // Convenience method for testing whether an employee
        // has an associated Compensation object.
        public bool EmployeeHasCompensation(string employeeId)
        {
            var compensation = _compensationRepository.GetByEmployeeId(employeeId);

            return compensation != null;
        }
    }

}
