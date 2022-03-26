using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeRegistration.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression("^[A-Za-z ]+$",ErrorMessage ="Only character allowed")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public Int32 Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [DataType(DataType.PhoneNumber)]
        [MinLength(10)]
        [MaxLength(10)]
        public string ContactNo { get; set; }

        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public string EmployeePhoto { get; set; }



        public List<EmployeeEducation> Educations { get; set; }
        public Employee()
        {
            Educations=new List<EmployeeEducation>();
        }
    }
}
