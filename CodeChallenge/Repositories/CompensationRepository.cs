using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public CompensationRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;

            // Consider dropping the logger dependency injection if
            // we won't be logging data from the repository. Included
            // here for consistency with the other repository classes.
            _logger = logger;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            // Lookup the compensation entity using the employee Id
            // which we are using as the compensation primary key.
            return _employeeContext.Compensations.SingleOrDefault(comp => comp.EmployeeId == employeeId);
        }

        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
