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
        public IEnumerable<Vehicle> GetAllWithPicsUser(int pageIndex, int pageSize, int rentServiceId, bool available, string price, int type)
        {
            if (available == true) {
                if (price == "Low") {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true).OrderBy(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true && x.TypeId==type).OrderBy(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                }
                if (price == "High")
                {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true).OrderByDescending(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true && x.TypeId == type).OrderByDescending(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    
                }
                else
                {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.Available == true && x.TypeId == type).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                }
            }
            else
            {
                if (price == "Low")
                {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true ).OrderBy(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.TypeId == type).OrderBy(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                }
                if (price == "High")
                {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true ).OrderByDescending(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true &&  x.TypeId == type).OrderByDescending(x => x.HourlyPrice).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }

                }
                else
                {
                    if (type == -1)
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true ).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return DemoContext.Vehicles.Include(s => s.VehiclePictures).Where(x => x.RentServiceId == rentServiceId && x.Enabled == true && x.TypeId == type).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                }
            }
        }
    
        public int CountServiceVehicles(int rentServiceId)
        {
            return DemoContext.Vehicles.Where(x => x.RentServiceId == rentServiceId).Count();
        }
    }
}