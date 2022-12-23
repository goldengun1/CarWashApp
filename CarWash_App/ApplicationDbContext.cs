using CarWash_App.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarWash_App
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>()
                .HasOne(x => x.CarWash)
                .WithMany(x => x.Services);

            modelBuilder.Entity<Service>()
                .HasOne(x => x.ServiceType)
                .WithMany(x => x.Services);

            modelBuilder.Entity<Service>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Services);


            //--------------------------------------//

            modelBuilder.Entity<ServiceType>()
                .HasMany(x => x.Services)
                .WithOne(x => x.ServiceType);

            //--------------------------------------//

            modelBuilder.Entity<CarWash>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.CarWashes);

            modelBuilder.Entity<CarWash>()
                .HasMany(x => x.Services)
                .WithOne(x => x.CarWash);

            //--------------------------------------//

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Services)
                .WithOne(x => x.Customer);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.CarWashes)
                .WithOne(x => x.Owner);

            modelBuilder.Entity<CarWashesServiceTypes>().HasKey(x => new { x.CarWashId,x.ServiceTypeId });

            //NOTE: see if this is necessary
            //modelBuilder.Entity<CarWashesServiceTypes>()
            //    .HasOne(x => x.CarWash)
            //    .WithMany(x => x.CarWashesServiceTypes)
            //    .HasForeignKey(x => x.CarWashId);

            //modelBuilder.Entity<CarWashesServiceTypes>()
            //    .HasOne(x => x.ServiceType)
            //    .WithMany(x => x.CarWashesServiceTypes)
            //    .HasForeignKey(x => x.ServiceTypeId);

            SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var password = "Password123!";

            var admin = new ApplicationUser()
            {
                Id = "838C5B52-9F4F-435E-809D-BD7D5864BB5E",
                FirstName = "Administrator",
                LastName = "Arministrator",
                UserName = "admin.admin",
                NormalizedUserName = "ADMIN.ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = false,
                PasswordHash = passwordHasher.HashPassword(null, password)
            };

            var users = new List<ApplicationUser>()
            {
                admin,
                new ApplicationUser()
                {
                    Id = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                    FirstName = "Mihailo",
                    LastName = "Simic",
                    IsAnOwner = true,
                    UserName = "mihailo.simic",
                    NormalizedUserName = "MIHAILO.SIMIC",
                    Email = "mihailo.simic@gmail.com",
                    NormalizedEmail = "MIHAILO.SIMIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)
                },
                new ApplicationUser()
                {
                    Id = "8F0A1391-5547-43F0-AEC9-59908CB381D9",
                    FirstName = "Stefanija",
                    LastName = "Markovic",
                    IsAnOwner= true,
                    UserName = "stefanija.markovic",
                    NormalizedUserName = "STEFANIJA.MARKOVIC",
                    Email = "stefanija.markovic@gmail.com",
                    NormalizedEmail = "STEFANIJA.MARKOVIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "4FE7601C-53C2-467B-8F0B-6AB8F048C680",
                    FirstName = "Pera",
                    LastName = "Peric",
                    IsAnOwner = true,
                    UserName = "pera.peric",
                    NormalizedUserName = "PERA.PERIC",
                    Email = "pera.peric@gmail.com",
                    NormalizedEmail = "PERA.PERIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "BB4077EC-96BD-4FF9-8365-F522AF43ED30",
                    FirstName = "Laza",
                    LastName = "Lazic",
                    IsAnOwner = true,
                    UserName = "laza.lazic",
                    NormalizedUserName = "LAZA.LAZIC",
                    Email = "laza.lazic@gmail.com",
                    NormalizedEmail = "LAZA.LAZIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "E8560300-CD67-45FB-B07F-7713F45D131A",
                    FirstName = "Mika",
                    LastName = "Mikic",
                    IsAnOwner = true,
                    UserName = "mika.mikic",
                    NormalizedUserName = "MIKA.MIKIC",
                    Email = "mika.mikic@gmail.com",
                    NormalizedEmail = "MIKA.MIKIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "722D96C6-60D1-4233-B41E-D788E2451D4F",
                    FirstName = "Ana",
                    LastName = "Anic",
                    IsAnOwner = false,
                    UserName = "ana.anic",
                    NormalizedUserName = "ANA.ANIC",
                    Email = "ana.anic@gmail.com",
                    NormalizedEmail = "ANA.ANIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "264249DB-C9DC-4827-A5D0-D7494C1086FD",
                    FirstName = "Jovan",
                    LastName = "Jovic",
                    IsAnOwner = false,
                    UserName = "jovan.jovic",
                    NormalizedUserName = "JOVAN.JOVIC",
                    Email = "jovan.jovic@gmail.com",
                    NormalizedEmail = "JOVAN.JOVIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "7949B386-70FB-403D-9692-8BE9DCF2BC1E",
                    FirstName = "Milovan",
                    LastName = "Milovanovic",
                    IsAnOwner = false,
                    UserName = "milovan.milovanovic",
                    NormalizedUserName = "MILOVAN.MILOVANOVIC",
                    Email = "milovan.milovanovic@gmail.com",
                    NormalizedEmail = "MILOVAN.MILOVANOVIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "193E7C46-91A2-4F45-9E11-44964496FB0F",
                    FirstName = "Petar",
                    LastName = "Petrovic",
                    IsAnOwner = false,
                    UserName = "petar.petrovic",
                    NormalizedUserName = "PETAR.PETROVIC",
                    Email = "petar.petrovic@gmail.com",
                    NormalizedEmail = "PETAR.PETROVIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                },
                new ApplicationUser()
                {
                    Id = "C3F0EF4D-A5A1-46CE-91CF-4443B95734C6",
                    FirstName = "Milan",
                    LastName = "Milanovic",
                    IsAnOwner = false,
                    UserName = "milan.milanovic",
                    NormalizedUserName = "MILAN.MILANOVIC",
                    Email = "milan.milanovic@gmail.com",
                    NormalizedEmail = "MILAN.MILANOVIC@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, password)

                }
            };

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            var carWashes = new List<CarWash>()
            {
                new CarWash()
                {
                    Id = 1,
                    CarWashName = "CarWashExtra",
                    OwnerId = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                    OpeningTime = 9,
                    ClosingTime = 17,
                    Profit = 0f
                },
                new CarWash()
                {
                    Id = 2,
                    CarWashName = "MegaWash",
                    OwnerId = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                    OpeningTime = 12,
                    ClosingTime = 22,
                    Profit = 0f
                },
                new CarWash()
                {
                    Id = 3,
                    CarWashName = "StecoPoint",
                    OwnerId = "8F0A1391-5547-43F0-AEC9-59908CB381D9",
                    OpeningTime = 6,
                    ClosingTime = 15,
                    Profit = 0f
                },
                new CarWash()
                {
                    Id = 4,
                    CarWashName = "4U2Wash",
                    OwnerId = "4FE7601C-53C2-467B-8F0B-6AB8F048C680",
                    OpeningTime = 0,
                    ClosingTime = 24,
                    Profit = 0f
                }
            };

            var carWashesServiceTypes = new List<CarWashesServiceTypes>()
            {
                new CarWashesServiceTypes(){ CarWashId = 1, ServiceTypeId = 1 },
                new CarWashesServiceTypes(){ CarWashId = 1, ServiceTypeId = 2 },
                new CarWashesServiceTypes(){ CarWashId = 1, ServiceTypeId = 3 },
                new CarWashesServiceTypes(){ CarWashId = 2, ServiceTypeId = 1 },
                new CarWashesServiceTypes(){ CarWashId = 2, ServiceTypeId = 2 },
                new CarWashesServiceTypes(){ CarWashId = 3, ServiceTypeId = 2 },
                new CarWashesServiceTypes(){ CarWashId = 3, ServiceTypeId = 3 },
                new CarWashesServiceTypes(){ CarWashId = 4, ServiceTypeId =  1}
            };

            var claims = new List<IdentityUserClaim<string>>()
            {
                new IdentityUserClaim<string>()
                {
                    Id = 11,
                    UserId = "838C5B52-9F4F-435E-809D-BD7D5864BB5E",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Admin"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 1,
                    UserId = "8F0A1391-5547-43F0-AEC9-59908CB381D9",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Owner"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 2,
                    UserId = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Owner"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 3,
                    UserId = "4FE7601C-53C2-467B-8F0B-6AB8F048C680",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Owner"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 4,
                    UserId = "BB4077EC-96BD-4FF9-8365-F522AF43ED30",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Owner"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 5,
                    UserId = "E8560300-CD67-45FB-B07F-7713F45D131A",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Owner"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 6,
                    UserId = "722D96C6-60D1-4233-B41E-D788E2451D4F",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Customer"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 7,
                    UserId = "264249DB-C9DC-4827-A5D0-D7494C1086FD",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Customer"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 8,
                    UserId = "7949B386-70FB-403D-9692-8BE9DCF2BC1E",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Customer"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 9,
                    UserId = "193E7C46-91A2-4F45-9E11-44964496FB0F",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Customer"
                },
                new IdentityUserClaim<string>()
                {
                    Id = 10,
                    UserId = "C3F0EF4D-A5A1-46CE-91CF-4443B95734C6",
                    ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ClaimValue = "Customer"
                }
            };


            modelBuilder.Entity<ApplicationUser>().HasData(users);
            modelBuilder.Entity<IdentityUserClaim<string>>().HasData(claims);
            modelBuilder.Entity<ServiceType>().HasData(serviceTypes);
            modelBuilder.Entity<CarWash>().HasData(carWashes);
            modelBuilder.Entity<CarWashesServiceTypes>().HasData(carWashesServiceTypes);


        }

        public DbSet<CarWash> carWashes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<CarWashesServiceTypes> CarWashesServiceTypes { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
    }

    
}
