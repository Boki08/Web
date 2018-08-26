using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class VehicleRepository : Repository<Vehicle, int>, IVehicleRepository
    {
        public VehicleRepository(DbContext context) : base(context)
        {
        }

        protected RADBContext DemoContext { get { return context as RADBContext; } }

        public IEnumerable<Vehicle> GetAllWithPics(int pageIndex, int pageSize,int rentServiceId)
        {
            return DemoContext.Vehicles.Include(s=>s.VehiclePictures).Where(x=>x.RentServiceId== rentServiceId).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public int CountServiceVehicles(int rentServiceId)
        {
            return DemoContext.Vehicles.Where(x => x.RentServiceId == rentServiceId).Count();
        }
    }
}