using LoanApp.Models;

namespace LoanApp.Data
{
    public interface ILoanApplicationRepository
    {
        Task<IEnumerable<LoanApplicationViewModel>> GetAll();
        Task<LoanApplicationViewModel> GetById(int id);
        Task<int> Create(LoanApplication model);
        Task UpdateStatus(int id, string status, string user);
        Task<IEnumerable<LoanType>> GetAllLoanTypes();
    }
}
