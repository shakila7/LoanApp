using System.ComponentModel.DataAnnotations;

namespace LoanApp.Models
{
    public class LoanType
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [Range(0.0, 100.0)]
        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }
    }
}
