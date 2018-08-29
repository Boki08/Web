using Microsoft.AspNet.Identity.EntityFramework;
using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance
{
    public class RADBContext : IdentityDbContext<RAIdentityUser>
    {
       // public virtual DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Comment> Comments { get; set; }
      
        public DbSet<Office> Offices { get; set; }
        public DbSet<OfficePicture> OfficePictures { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RentService> RentServices { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehiclePicture> VehiclePictures { get; set; }
        public DbSet<TypeOfVehicle> TypesOfVehicles { get; set; }



        public RADBContext() : base("name=RADB")
        {
        }

        public static RADBContext Create()
        {
            return new RADBContext();
        }
    }
}