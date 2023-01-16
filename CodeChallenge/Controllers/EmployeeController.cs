using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        /// <summary>
        /// Gets the total number of reports (not just the number of 
        /// direct reports) for the employee with the supplied id.
        /// </summary>
        /// <param name="id">The employee id of the employee to interrogate.</param>
        /// <returns>The total number of reports for the employee with the supplied id.</returns>
        [HttpGet("{id}/numberOfReports")]
        public IActionResult GetNumberOfReports(String id)
        {
            _logger.LogDebug($"Received Number of Reports GET request for employee with '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            int totalReports = _employeeService.GetNumberOfReports(id);

            var reportingStructure = new ReportingStructure() {
                employee = employee,
                numberOfReports = totalReports
            };

            // TODO: Determine desired behavior for mapping navigation
            // properties to JSON output. Currently GetEmployeeById() does
            // NOT fufill its contract as it returns either null for the 
            // DirectReports property if EntityFramework has not yet loaded
            // the navigation property, or an array of complete Employee
            // objects representing the direct reports if EntityFramework
            // has loaded the property.

            // The specification (ReadMe.md) indicates the DirectReports property should
            // be a string array of (presumably) the direct reports' employee Ids.

            // Here we create an anonymously typed DTO object
            // for the ReportingStructure, in order to map the list of
            // DirectReport employees to a single array of strings
            // (the direct reports' employee Ids.) For more complex APIs, a
            // separate Dto class is advisble, possibly with the use of
            // a mapping library such as AutoMapper (http://automapper.org/).
            var reportingStructureDto = new
            {
                employee = new
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position = employee.Position,
                    Department = employee.Department,
                    DirectReports = employee.DirectReports.Select(report => report.EmployeeId)
                },
                numberOfReports = totalReports
            };

            // Return the DTO we created
            return Ok(reportingStructureDto);

        }

        [HttpGet("{employeeId}/compensation", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String employeeId)
        {
            _logger.LogDebug($"Received compensation get request for employee with Id '{employeeId}'");

            // Force a lookup of the full employee object associated
            // with this compensation, to populate the navigation property
            var employee = _employeeService.GetById(employeeId);

            // Lookup the compensation entity associated with
            // the supplied employeeId.
            var compensation = _employeeService.GetCompensationByEmployeeId(employeeId);

            // If an associated compensation entity could not be found
            // return an appropriate status error.
            if (compensation == null)
            {
                return NotFound("Error: The specified employee could not be found.");
            }

            return Ok(compensation);
        }

        [HttpPost("compensation")]
        public IActionResult AddCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for employee with Id '{compensation.EmployeeId}'");

            // Force a lookup of the full employee entity associated
            // with this compensation, to populate the navigation property
            var employee = _employeeService.GetById(compensation.EmployeeId);

            if (employee == null)
            {
                // If the employee could not be found
                // return an appropriate 404 error.
                return NotFound("Error: The specified employee could not be found.");
            }

            if (_employeeService.EmployeeHasCompensation(employee.EmployeeId))
            {
                // If the employee already has an associated compensation entity
                // return an appropriate 409 error.
                return Conflict("Error: The specified employee already has associated compensation.");
            }

            // Persist the new compensation entity.
            _employeeService.AddCompensation(compensation);

            // Return a Created 201 response with a header link to
            // the newly created compensation's get method.
            return CreatedAtRoute("getCompensationByEmployeeId", new { employeeId = compensation.EmployeeId }, compensation);

        }
    }
}
