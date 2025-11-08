using LoanApp.Models;

namespace LoanApp.Data
{
    public interface ILoanRepository
    {
        Task<IEnumerable<LoanApplication>> GetAll();
        Task<LoanApplication> GetById(int id);
        Task<int> Create(LoanApplication model);
        Task UpdateStatus(int id, string status);
    }
}
