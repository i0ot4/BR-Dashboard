using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class NeighborhoodController : Controller
    {
        private readonly INeighborhoodService neighborhoodService;
        private readonly ICityRepository cityRepository;
        private readonly IDirectorateRepository directorateRepository;
        private readonly IUnitOfWork unitOfWork;
        public NeighborhoodController(
            INeighborhoodService neighborhoodService,
            ICityRepository cityRepository,
            IDirectorateRepository directorateRepository,
            IUnitOfWork unitOfWork)
        {
            this.neighborhoodService = neighborhoodService;
            this.cityRepository = cityRepository;
            this.directorateRepository = directorateRepository; 
            this.unitOfWork = unitOfWork;
        }       

        public async Task<IActionResult> Index()
        {
            try
            {
                var getItems = await neighborhoodService.GetAll(p => p.IsDeleted == false);
                if(getItems.Succeeded && getItems.Data != null)
                {
                    return View(getItems.Data);
                }
                TempData["error"] = $"{getItems.Status.message}";
                return View();
            }catch(Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
            
        }

        

        public async Task<IActionResult> Add()
        {
            try
            {

                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p=>p.IsDeleted == false), "Id","CityName");                
                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p=>p.IsDeleted == false), "Id", "DirectorateName");                
                return View();
            }
            catch (Exception ex)
            {

                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Neighborhood obj)
        {
            try
            {
                if (obj != null)
                {
                    
                    obj.IsDeleted = false;
                    if(obj.NeighborhoodName == null ||  obj.NeighborhoodName.Length == 0)
                    {
                        ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                        ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                        return View(obj);
                    }
                    
                    var addItem = await neighborhoodService.Add(obj);
                    if (addItem.Succeeded)
                    {
                        TempData["msg"] = "تمت الاضافة بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["error"] = $"{addItem.Status.message}";
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                    ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                    return View(obj);
                }
                else
                {
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                    ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                    TempData["error"] = "not be null";
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
                var upRes = await neighborhoodService.ChangeActive(id, !isActive);
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id)
        {

            try
            {
                if (id == 0)
                {
                    TempData["error"] = $"يرجى اختيار عنصر صحيحة لتعديلها";
                    return RedirectToAction(nameof(Index));
                }
                var getItem = await neighborhoodService.GetForUpdate(id);
                if (getItem.Succeeded && getItem.Data != null)
                {

                    return View(getItem.Data);
                }

                TempData["error"] = $"يرجى اختيار عنصر صحيحة لتعديلها";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Neighborhood obj)
        {
            try
            {

                if (obj == null)
                {
                    return View(obj);
                }
                if(obj.NeighborhoodName == null || obj.NeighborhoodName.Length == 0)
                {
                    return View(obj);
                }
                var editItem = await neighborhoodService.Update(obj);
                if (editItem.Succeeded)
                {
                    TempData["msg"] = "تمت العملية بنجاح";
                    return RedirectToAction(nameof(Index));
                }

                TempData["error"] = $"{editItem.Status.message}";
                return View(obj);

            }
            catch (Exception exp)
            {
                TempData["error"] = $"{exp.Message}";
                return View(obj);
            }
        }
    }
}
