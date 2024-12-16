using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BR.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly ICityService cityService;
        private readonly IUnitOfWork unitOfWork;
        public CityController(ICityService cityService, IUnitOfWork unitOfWork)
        {
            this.cityService = cityService;
            this.unitOfWork = unitOfWork;
        }       

        public async Task<IActionResult> Index()
        {
            try
            {
                var getItems = await cityService.GetAll(p => p.IsDeleted == false);
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

        

        public IActionResult Add()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(City obj)
        {
            try
            {
                if (obj != null)
                {
                    
                    obj.IsDeleted = false;
                    if(obj.CityName == null ||  obj.CityName.Length == 0)
                    {
                        return View(obj);
                    }
                    
                    var addItem = await cityService.Add(obj);
                    if (addItem.Succeeded)
                    {
                        TempData["msg"] = "تمت الاضافة بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
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
                var upRes = await cityService.ChangeActive(id, !isActive);
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
                var getItem = await cityService.GetForUpdate(id);
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
        public async Task<IActionResult> Edit(City obj)
        {
            try
            {

                if (obj == null)
                {
                    return View(obj);
                }
                if(obj.CityName == null || obj.CityName.Length == 0)
                {
                    return View(obj);
                }
                var editItem = await cityService.Update(obj);
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
