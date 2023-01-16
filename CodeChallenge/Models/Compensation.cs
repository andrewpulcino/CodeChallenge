using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    public class Compensation
    {

        // Foreign key referencing the related employee
        // We'll also use this as the primary key for
        // our Compensation object, as compensations and
        // employees are related one-to-one.
        [Required]
        [Key]
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }

        // Reference navigation property
        public Employee Employee { get; set; }

        public decimal Salary { get; set; }

        public DateOnly EffectiveDate { get; set; } 

    }
}
