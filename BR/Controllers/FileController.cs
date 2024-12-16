using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISysUserRepository sysUserRepository;
        private readonly IPreviousWorkRepository previousWorkRepository;
        private readonly IPreviousWorkImageRepository previousWorkImageRepository;
        public FileController(
            IWebHostEnvironment env,
            ISysUserRepository sysUserRepository,
            IPreviousWorkRepository previousWorkRepository,
            IPreviousWorkImageRepository previousWorkImageRepository
            )
        {
            this.env = env;
            this.sysUserRepository = sysUserRepository;
            this.previousWorkRepository = previousWorkRepository;
            this.previousWorkImageRepository = previousWorkImageRepository;
        }
        [HttpPost("UploadImage")]
        public async Task<IResult<SysUser>> UploadImage([FromForm] UserImage model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return await Result<SysUser>.FailAsync("user not found");
            }
            try
            {
                var result = await sysUserRepository.GetById(model.UserId);
                if (result == null)
                {
                    return await Result<SysUser>.FailAsync("user not found");

                }
                var image = await SaveFile(model.Image);
                if (image == null)
                {
                    return await Result<SysUser>.FailAsync("can not save iamge");

                }
                result.Image = image;
                sysUserRepository.Update(result);
                sysUserRepository.SaveAsync();
                return await Result<SysUser>.SuccessAsync("done");

            }
            catch (Exception ex)
            {
                return await Result<SysUser>.FailAsync("can not save update operation");
            }
            
        }
        [HttpPost("UploadCommericalImage")]
        public async Task<IResult<SysUser>> CommericalImage([FromForm] CommericalImage model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return await Result<SysUser>.FailAsync("user not found");
            }
            try
            {
                var result = await sysUserRepository.GetById(model.UserId);
                if (result == null)
                {
                    return await Result<SysUser>.FailAsync("user not found");

                }
                var CommericalRegistrationImage = await SaveFile(model.CommericalRegistrationImage);
                if (CommericalRegistrationImage == null)
                {
                    return await Result<SysUser>.FailAsync("can not save Commerical Registration Image");

                }
                var image = await SaveFile(model.Image);
                if (image == null)
                {
                    return await Result<SysUser>.FailAsync("can not save iamge");

                }
                result.CommericalRegistrationImage = CommericalRegistrationImage;
                result.Image = image;
                sysUserRepository.Update(result);
                sysUserRepository.SaveAsync();
                return await Result<SysUser>.SuccessAsync("done");

            }
            catch (Exception ex)
            {
                return await Result<SysUser>.FailAsync("can not save update operation");
            }

        }
        [HttpPost("UploadPreviousWorkImage")]
        public async Task<IResult<int>> PreviousWorkImage([FromForm] PreviousWorkImageVM model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return await Result<int>.FailAsync("user not found");
            }
            try
            {
                var result = await sysUserRepository.GetById(model.UserId);
                if (result == null)
                {
                    return await Result<int>.FailAsync("user not found");

                }
                var previousWork = await previousWorkRepository.GetById(model.PrevoiusWorkId);
                if (previousWork == null)
                {
                    return await Result<int>.FailAsync("previous work not found");

                }
                var Image = await SaveFile(model.Image);
                if (Image == null)
                {
                    return await Result<int>.FailAsync("can not save Commerical Registration Image");

                }
                PreviousWorkImage previousWorkImage = new PreviousWorkImage
                {
                    ImageTitle = Image,
                    ImagePath = Image,
                    PreviousWorkId = model.PrevoiusWorkId,
                };
                previousWorkImageRepository.Add(previousWorkImage);
                previousWorkImageRepository.SaveAsync();
                if(model.Image2 != null) {
                    var image2 = await SaveFile(model.Image2);
                    if (image2 == null)
                    {
                        return await Result<int>.FailAsync("can not save iamge 2");

                    }
                    PreviousWorkImage previousWorkImage2 = new PreviousWorkImage
                    {
                        ImageTitle = image2,
                        ImagePath = image2,
                        PreviousWorkId = model.PrevoiusWorkId,
                    };
                    previousWorkImageRepository.Add(previousWorkImage2);
                    previousWorkImageRepository.SaveAsync();
                }
                
                if (model.Image3 != null)
                {
                    var image3 = await SaveFile(model.Image3);
                    if (image3 == null)
                    {
                        return await Result<int>.FailAsync("can not save iamge 3");
                    }
                    PreviousWorkImage previousWorkImage3 = new PreviousWorkImage
                    {
                        ImageTitle = image3,
                        ImagePath = image3,
                        PreviousWorkId = model.PrevoiusWorkId,
                    };
                    previousWorkImageRepository.Add(previousWorkImage3);
                    previousWorkImageRepository.SaveAsync();
                }
                return await Result<int>.SuccessAsync("done");

            }
            catch (Exception ex)
            {
                return await Result<int>.FailAsync("can not save update operation");
            }

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
