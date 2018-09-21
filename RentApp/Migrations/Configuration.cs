namespace RentApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using RentApp.Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<RentApp.Persistance.RADBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RentApp.Persistance.RADBContext context)
        {
            //debug
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
            //((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors




            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //if (!context.Roles.Any(r => r.Name == "Admin"))
            //{
            //    var store = new RoleStore<IdentityRole>(context);
            //    var manager = new RoleManager<IdentityRole>(store);
            //    var role = new IdentityRole { Name = "Admin" };

            //    manager.Create(role);
            //}

            //if (!context.Roles.Any(r => r.Name == "Manager"))
            //{
            //    var store = new RoleStore<IdentityRole>(context);
            //    var manager = new RoleManager<IdentityRole>(store);
            //    var role = new IdentityRole { Name = "Manager" };

            //    manager.Create(role);
            //}

            //if (!context.Roles.Any(r => r.Name == "AppUser"))
            //{
            //    var store = new RoleStore<IdentityRole>(context);
            //    var manager = new RoleManager<IdentityRole>(store);
            //    var role = new IdentityRole { Name = "AppUser" };

            //    manager.Create(role);
            //}

            //context.AppUsers.AddOrUpdate(

            //      u => u.FullName,

            //      new AppUser() { FullName = "Admin Adminovic" }

            //);

            //context.AppUsers.AddOrUpdate(

            //    p => p.FullName,

            //    new AppUser() { FullName = "AppUser AppUserovic" }

            //);

            //context.SaveChanges();

            //var userStore = new UserStore<RAIdentityUser>(context);
            //var userManager = new UserManager<RAIdentityUser>(userStore);

            //if (!context.Users.Any(u => u.UserName == "admin"))
            //{
            //    var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Admin Adminovic");
            //    var user = new RAIdentityUser() { Id = "admin", UserName = "admin", Email = "admin@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("admin"), AppUserId = _appUser.Id };
            //    userManager.Create(user);
            //    userManager.AddToRole(user.Id, "Admin");
            //}

            //if (!context.Users.Any(u => u.UserName == "appu"))

            //{

            //    var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "AppUser AppUserovic");
            //    var user = new RAIdentityUser() { Id = "appu", UserName = "appu", Email = "appu@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("appu"), AppUserId = _appUser.Id };
            //    userManager.Create(user);
            //    userManager.AddToRole(user.Id, "AppUser");

            //}

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Manager"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Manager" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "AppUser"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "AppUser" };

                manager.Create(role);
            }

            context.AppUsers.AddOrUpdate(

                  u => u.FullName,

                  new AppUser() { FullName = "Admin Adminovic",  Email = "admin@yahoo.com",  BirthDate=DateTime.Parse("1/1/2000"),DocumentPicture="123" }

            );
            context.AppUsers.AddOrUpdate(

                  u => u.FullName,

                  new AppUser() { FullName = "Manager", Email = "manager@yahoo.com", BirthDate = DateTime.Parse("1/1/2000"), DocumentPicture = "123" }

            );
            context.AppUsers.AddOrUpdate(

                p => p.FullName,

                new AppUser() { FullName = "AppUser AppUserovic", Email = "user@yahoo.com", BirthDate = DateTime.Parse("1/1/2001"), DocumentPicture = "11" }

            );

            //rentServices

            context.RentServices.AddOrUpdate(

                p => p.Name,

                new RentService() { Name = "Rent1", Email = "rent1@yahoo.com", Description="Description of the description",Grade=4,Logo="1234" }

            );
            context.RentServices.AddOrUpdate(

                p => p.Name,

                new RentService() { Name = "Rent2", Email = "rent2@yahoo.com", Description = "Description of the description", Grade = 1, Logo = "1234" }

            );
            context.RentServices.AddOrUpdate(

               p => p.Name,

               new RentService() { Name = "Rent3", Email = "rent3@yahoo.com", Description = "Description of the description", Grade = 5, Logo = "1234" }

           );
            //type
           // context.TypesOfVehicles.AddOrUpdate(

           //     p => p.TypeId,

           //     new TypeOfVehicle() { Type = "type1" }

           // ); context.TypesOfVehicles.AddOrUpdate(

           //      p => p.TypeId,

           //      new TypeOfVehicle(){ Type = "type1" }

           //  ); context.TypesOfVehicles.AddOrUpdate(

           //      p => p.TypeId,

           //      new TypeOfVehicle() { Type = "type1" }

           //  );
           // context.Vehicles.AddOrUpdate(

           //     p => p.VehicleId,

           //     new Vehicle()
           //     {
           //         Available = true,
           //         Manufacturer = "rent1@yahoo.com",
           //         Model = "Description of the description",
           //         Description = "jdfgdhfdhgf",
           //         Pictures = "1234",
           //         RentServiceId = 1,
           //         YearOfManufacturing = "2010",
           //         TypeId = 1,
           //     }
                

           // );
           // context.Vehicles.AddOrUpdate(

           //     p => p.VehicleId,

           //     new Vehicle()
           //     {
           //         Available = true,
           //         Manufacturer = "rent1@yahoo.com",
           //         Model = "Description of the description2",
           //         Description = "jdfgdhfdhgf",
           //         Pictures = "1234",
           //         RentServiceId = 1,
           //         YearOfManufacturing = "2015",
           //         TypeId = 2,
           //     }
           // );
           // context.Vehicles.AddOrUpdate(

           //    p => p.VehicleId,

           //    new Vehicle()
           //    {
           //        Available = false,
           //        Manufacturer = "rent1@yahoo.com",
           //        Model = "Description of the description3",
           //        Description = "jdfgdhfdhgf",
           //        Pictures = "1234",
           //        RentServiceId = 2,
           //        YearOfManufacturing = "2013",
           //       TypeId = 3,
           //    }
           //);







            try
            {
                context.SaveChanges();
            }
            //catch (DbEntityValidationException ex)
            //{
            //    StringBuilder sb = new StringBuilder();

            //    foreach (var failure in ex.EntityValidationErrors)
            //    {
            //        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
            //        foreach (var error in failure.ValidationErrors)
            //        {
            //            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
            //            sb.AppendLine();
            //        }
            //    }

            //    throw new DbEntityValidationException(
            //        "Entity Validation Failed - errors follow:\n" +
            //        sb.ToString(), ex
            //    ); // Add the original exception as the innerException
            //}
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            // context.SaveChanges();


            var userStore = new UserStore<RAIdentityUser>(context);
            var userManager = new UserManager<RAIdentityUser>(userStore);

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Admin Adminovic");
                var user = new RAIdentityUser() { Id = "admin", UserName = "admin", Email = "admin@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("admin"), AppUserId = _appUser.UserId };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");
            }
            if (!context.Users.Any(u => u.UserName == "manager"))
            {
                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Manager");
                var user = new RAIdentityUser() { Id = "manager", UserName = "manager", Email = "menager@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("manager"), AppUserId = _appUser.UserId };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Manager");
            }

            if (!context.Users.Any(u => u.UserName == "appu"))

            {

                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "AppUser AppUserovic");
                var user = new RAIdentityUser() { Id = "appu", UserName = "appu", Email = "appu@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("appu"), AppUserId = _appUser.UserId };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "AppUser");

            }
        }
    }
}
