using BR.Models;
using BR.Models.VMs;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class PublicationController : Controller
    {
        private readonly IPublicationService publicationService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISysUserRepository sysUserRepository;

        public PublicationController(IPublicationService publicationService, IUnitOfWork unitOfWork, ISysUserRepository sysUserRepository)
        {
            this.publicationService = publicationService;
            this.unitOfWork = unitOfWork;
            this.sysUserRepository = sysUserRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                
                var getItems = await publicationService.GetAllWithOwner(p=>p.IsDeleted == false);

                if (getItems.Succeeded && getItems.Data != null)
                {
                    return View(getItems.Data);
                }
                TempData["error"] = $"{getItems.Status.message}";
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                
                ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(PublicationVM obj)
        {
            try
            {
                if (obj != null)
                {
                    
                    if (obj.Title == null || obj.Title.Length == 0)
                    {
                        ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                        return View(obj);
                    }
                    var publication = new Publication
                    {
                        Title = obj.Title,
                        Content = obj.Content,
                        UserId = obj.UserId,
                        PhoneNumber = obj.PhoneNumber,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        IsActive = true
                    };
                    var addItem = await publicationService.Add(publication);
                    if (addItem.Succeeded)
                    {
                        TempData["msg"] = "تمت الاضافة بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                    TempData["error"] = $"{addItem.Status.message}";
                    return View(obj);
                }
                else
                {
                    TempData["error"] = "image can not be null";
                    return View(obj);
                }


            }
            catch (Exception exp)
            {
                TempData["error"] = $"{exp.Message}";
                return View(obj);
            }
        }

        public async Task<ActionResult> ChangeIsActive(long id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await publicationService.ChangeActive(id, !isActive);
                if (upRes.Succeeded)
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
