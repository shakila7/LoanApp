using LoanApp.Data;
using LoanApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanRepository _loanRepository;
        public LoanController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<LoanApplication> list = _loanRepository.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new LoanApplication { Status = "New", InterestRate = 12.5m };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoanApplication model)
        {
            if (!ModelState.IsValid) return View(model);
            _loanRepository.Create(model);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var m = _loanRepository.GetById(id);
            if (m == null) return NotFound();
            return View(m);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string status)
        {
            var m = _loanRepository.GetById(id);
            if (m == null) return NotFound();
            if (string.IsNullOrWhiteSpace(status))
            {
                ModelState.AddModelError("Status", "Status is required");
                return View(m);
            }
            _loanRepository.UpdateStatus(id, status);
            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            var m = _loanRepository.GetById(id);
            if (m == null) return NotFound();
            return View(m);
        }
    }
}
