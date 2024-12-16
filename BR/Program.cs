using BR.Data;
using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using BR.Repositories;
using BR.Services.IServices;
using BR.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
    .AddDataAnnotationsLocalization();
builder.Services.AddControllers();
//localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options => {
    var supportedCultures = new[] { "en-US", "ar-YE" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

});
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddSingleton<ISharedViewLocalizer, SharedViewLocalizer>();

//session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IOTimeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ISessionHelper, SessionHelper>();

//dbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<SysUser, IdentityRole>(
    options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    }
    ).AddEntityFrameworkStores<ApplicationDbContext>();


//repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISysUserRepository, SysUserRepository>();
builder.Services.AddScoped<IConstructionShopRepository, ConstructionShopRepository>();
builder.Services.AddSingleton<ISharedViewLocalizer, SharedViewLocalizer>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IDirectorateRepository, DirectorateRepository>();
builder.Services.AddScoped<INeighborhoodRepository, NeighborhoodRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IPublicationRepository, PublicationRepository>();
builder.Services.AddScoped<IPreviousWorkRepository, PreviousWorkRepository>();
builder.Services.AddScoped<IPreviousWorkImageRepository, PreviousWorkImageRepository>();
builder.Services.AddScoped<IAccountEditRequestRepository, AccountEditRequestRepository>();
builder.Services.AddScoped<IFactoryRepository, FactoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IContractorWorkerRepository, ContractorWorkerRepository>();
builder.Services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));

builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDirectorateService, DirectorateService>();
builder.Services.AddScoped<INeighborhoodService, NeighborhoodService>();
builder.Services.AddScoped<IPublicationService, PublicationService>();
builder.Services.AddScoped<IPreviousWorkService, PreviousWorkService>();
builder.Services.AddScoped<IContractorWorkerService, ContractorWorkerService>();
builder.Services.AddScoped<IPreviousWorkImageService, PreviousWorkImageService>();

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
