using BR.helper;
using BR.Models;
using BR.Models.VMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;

namespace BR.Data
{
    public class ApplicationDbContext : IdentityDbContext<SysUser, IdentityRole<string>, string>
    {
        private readonly ISessionHelper session;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ISessionHelper session,
            IHttpContextAccessor httpContextAccessor
            ) : base(options)
        {
            this.session = session;
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AccountDeleteRequest> AccountDeleteRequests { get; set; }
        public DbSet<AccountModificationRequest> AccountModificationRequests { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Directorate> Directorates { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<PreviousWork> PreviousWorks { get; set; }
        public DbSet<PreviousWorkImage> PreviousWorkImages { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<AccountEditRequest> AccountEditRequests { get; set; }
        public DbSet<ContractorWorkers> ContractorWorkers { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserName = !string.IsNullOrEmpty(userId) ? userId : "Anonymous";

            foreach (var entry in ChangeTracker.Entries<SysUser>()) {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = $"{currentUserName}"; //_currentUserService.UserId ?? "1";
                        entry.Entity.CreatedOn = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = $"{currentUserName}"; //_currentUserService.UserId;
                        entry.Entity.ModifiedOn = DateTime.Now;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedBy = $"{currentUserName}";
                        entry.Entity.ModifiedOn = DateTime.Now;
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<TraceEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = $"{currentUserName}"; //_currentUserService.UserId ?? "1";
                        entry.Entity.CreatedOn = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = $"{currentUserName}"; //_currentUserService.UserId;
                        entry.Entity.ModifiedOn = DateTime.Now;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedBy = $"{currentUserName}";
                        entry.Entity.ModifiedOn = DateTime.Now;
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //create default admin user
            string ADMIN_ID = "02174cf0–9412–4cfe - afbf - 59f706d72cf6";
            string ROLE_ID = "341743f0 - asd2–42de - afbf - 59kmkkmk72cf6";

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Id = ROLE_ID,
                ConcurrencyStamp = ROLE_ID
            });

            var appUser = new SysUser
            {
                Id = ADMIN_ID,
                UserName = "778452083",
                PhoneNumber = "778452083",
                Email = "superadmin@gmail.com",
                FullName = "Administrator",
                NormalizedUserName = "778452083",
                NormalizedEmail = "SUPERADMIN@GMAIL.COM",
                UserType = "SuperAdmin",
                EmailConfirmed = true,
                IsConfirmed = true,
                IsActive = true,

            };

            PasswordHasher<SysUser> ph = new PasswordHasher<SysUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "Pa$$w0rd");
            builder.Entity<SysUser>().HasData(appUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });

            //relationships
            builder.Entity<SysUser>()
            .HasMany(a => a.Publications) 
            .WithOne(b => b.SysUser)
            .HasForeignKey(b => b.UserId);

            builder.Entity<SysUser>()
            .HasMany(a => a.PreviousWork)
            .WithOne(b => b.SysUser)
            .HasForeignKey(b => b.UserId);

            builder.Entity<PreviousWork>()
            .HasMany(a => a.PreviousWorkImages)
            .WithOne(b => b.PreviousWork)
            .HasForeignKey(b => b.PreviousWorkId);
            //AccountDeleteRequest config
            builder.Entity<AccountDeleteRequest>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<AccountDeleteRequest>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<AccountDeleteRequest>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //AccountModificationRequest
            builder.Entity<AccountModificationRequest>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<AccountModificationRequest>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<AccountModificationRequest>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //City
            builder.Entity<City>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<City>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<City>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //Directorate
            builder.Entity<Directorate>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<Directorate>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<Directorate>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //Neighborhood
            builder.Entity<Neighborhood>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<Neighborhood>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<Neighborhood>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //PreviousWork
            builder.Entity<PreviousWork>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<PreviousWork>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<PreviousWork>().Property(e => e.IsDeleted).HasDefaultValue(false);


            //PreviousWorkImage
            builder.Entity<PreviousWorkImage>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<PreviousWorkImage>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<PreviousWorkImage>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //Publication
            builder.Entity<Publication>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<Publication>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<Publication>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //UserRating
            builder.Entity<UserRating>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<UserRating>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<UserRating>().Property(e => e.IsDeleted).HasDefaultValue(false);

            //SysUser
            builder.Entity<SysUser>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<SysUser>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<SysUser>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<SysUser>().Property(x => x.Image).HasColumnName(@"Image").HasColumnType("nvarchar(255)").IsRequired(false).HasMaxLength(255);
            //ContractorWorkers
            builder.Entity<ContractorWorkers>().Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            builder.Entity<ContractorWorkers>().Property(e => e.IsActive).HasDefaultValue(false);
            builder.Entity<ContractorWorkers>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<ContractorWorkers>().Property(x => x.Image).HasColumnName(@"Image").HasColumnType("nvarchar(255)").IsRequired(false).HasMaxLength(255);


        }
    }
}
