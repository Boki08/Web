using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class RentServiceRepository : Repository<RentService, int>, IRentServiceRepository
    {
        public RentServiceRepository(DbContext context) : base(context)
        {
        }

       



        protected RADBContext DemoContext { get { return context as RADBContext; } }

        public RentService GetServiceWithOrders(int serviceId)
        {
            return (from service in DemoContext.RentServices
              from vehicle in service.Vehicles
              from order in vehicle.Orders
              where order.VehicleId == vehicle.VehicleId && vehicle.RentServiceId==service.RentServiceId
              select service).Include("Orders").ToList().FirstOrDefault();
           // return DemoContext.RentServices.Include(s=>s.Vehicles).t
             //   .Where(x => x.RentServiceId == serviceId).FirstOrDefault();
        }
       
    }
}