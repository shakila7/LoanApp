using LoanApp.Models;

namespace LoanApp.Data
{
    public interface ILoanRepository
    {
        IEnumerable<LoanApplication> GetAll();
        LoanApplication GetById(int id);
        int Create(LoanApplication model);
        void UpdateStatus(int id, string status);
    }
}
