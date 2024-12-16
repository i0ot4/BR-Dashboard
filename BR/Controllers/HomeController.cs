using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment env;

        private readonly ISysUserRepository sysUserRepository;
        public HomeController(ILogger<HomeController> logger,IWebHostEnvironment env, ISysUserRepository sysUserRepository)
        {
            _logger = logger;
            this.sysUserRepository = sysUserRepository;
            this.env = env;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            try
            {
                var result = await sysUserRepository.GetAll(x=>x.IsConfirmed == false);
                if (result.Any() && result.Count() > 0)
                {
                    return View(result);
                }
                TempData["msg"] = "لا يوجد بيانات";
                return View();

            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View("index", "home");
            }
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpPost("UploadImage")]
        public async Task<IResult<bool>> UploadImage([FromForm] IFormFile image, string location)
        {
            var result = await SaveFile(image, location);
            if (string.IsNullOrEmpty(result))
            {
                return  await Result<bool>.SuccessAsync(result);
            }
            return await Result<bool>.SuccessAsync(result);
        }
        public async Task<string> SaveFile(IFormFile file, string savePath = "AllFiles", int fileType = 0)
        {
            try
            {
                string fname = "";
                string unqName = "";
                string ext = "";
                if (file != null && file.Length > 0)
                {
                    //string tempFileName = Path.GetTempFileName();
                    ext = Path.GetExtension(file.FileName);
                    string attFolder = Path.Combine(env.WebRootPath, savePath);
                    if (!Directory.Exists(attFolder))
                    {
                        Directory.CreateDirectory(attFolder);
                    }
                    unqName = string.Format("{0}{1}", DateTime.Now.ToString("ddmmyyhhmmssfff"), ext); //Guid.NewGuid().ToString().Substring(0, 10) + ext);

                    fname = Path.Combine(attFolder, unqName);
                    using (FileStream str = new FileStream(fname, FileMode.Create))
                    {
                        file.CopyTo(str);
                    }

                    return Path.Combine(savePath, unqName);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

    }
}