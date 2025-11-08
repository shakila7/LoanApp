using LoanApp.Data;
using LoanApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace LoanApp.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanApplicationRepository _loanRepository;
        public LoanController(ILoanApplicationRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<LoanApplicationViewModel> list = await _loanRepository.GetAll();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new LoanApplication();
            await PopulateDropDowns();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanApplication model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDowns();
                return View(model);
            }
            model.CreatedBy = User.Identity?.Name ?? "System";
            model.UpdatedBy = User.Identity?.Name ?? "System";
            await _loanRepository.Create(model);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var m = await _loanRepository.GetById(id);
            if (m == null) return NotFound();
            return View(m);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string status)
        {
            var m = await _loanRepository.GetById(id);
            if (m == null) return NotFound();
            if (string.IsNullOrWhiteSpace(status))
            {
                ModelState.AddModelError("Status", "Status is required");
                return View(m);
            }
            var user = User.Identity?.Name ?? "System";

            await _loanRepository.UpdateStatus(id, status, user);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int id)
        {
            var m = await _loanRepository.GetById(id);
            if (m == null) return NotFound();
            return View(m);
        }
        private async Task PopulateDropDowns()
        {
            var loanTypes = await _loanRepository.GetAllLoanTypes();
            ViewBag.LoanTypes = loanTypes.Select(x => new
            {
                x.Id,
                x.Name,
                x.InterestRate
            }).ToList();
        }
    }
}
