using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BR.Controllers
{
    public class AccountEditRequestController : Controller
    {
        private readonly IAccountEditRequestRepository accountEditRequestRepository;
        private readonly IUnitOfWork unitOfWork;
        public AccountEditRequestController(IAccountEditRequestRepository accountEditRequestRepository, IUnitOfWork unitOfWork)
        {
            this.accountEditRequestRepository = accountEditRequestRepository;
            this.unitOfWork = unitOfWork;
        }       

        public async Task<IActionResult> Index()
        {
            try
            {
                var getItems = await accountEditRequestRepository.GetAll(p => p.IsDeleted == false && p.IsConfirmed == false);

                
                if(getItems != null)
                {
                    return View(getItems);
                }
                TempData["error"] = "error!";
                return View();
            }catch(Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
            
        }

        

        
        public async Task<ActionResult> ChangeIsActive(long id, string accountId)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await accountEditRequestRepository.AllowEdit(accountId, id);
                if (upRes)
                {
                    //TempData["msg"] = upRes.Status.message;
                    return RedirectToAction(nameof(Index));
                }
                //TempData["error"] = upRes.Status.message;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

       
    }
}
