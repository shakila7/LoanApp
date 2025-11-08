using System.ComponentModel.DataAnnotations;

namespace LoanApp.Models
{
    public class LoanApplication
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(200)]
        public string CustomerName { get; set; }


        [Required]
        [Display(Name = "NIC/Passport Number")]
        [StringLength(50)]
        public string NicPassport { get; set; }


        [Required]
        [Display(Name = "Loan Type")]
        public string LoanType { get; set; }


        [Required]
        [Range(0.0, 100.0)]
        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }


        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Loan Amount must be greater than 0")]
        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }


        [Required]
        [Range(1, 12, ErrorMessage = "Duration must be in months (1-12)")]
        [Display(Name = "Duration (Months)")]
        public int DurationMonths { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
