using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EmployeeRegistration.Models
{
    public class EmployeeEducation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        
        public int EmployeeId { get; set; }


        [Required]
        public string DegreeName { get; set; }

        [Required]
        public string yearOfPassing { get; set; }

        [Required]
        [Range(0,100)]
        public Decimal Percentage { get; set; }

    }
}
