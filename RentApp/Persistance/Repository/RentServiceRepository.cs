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

        public IEnumerable<RentService> GetAllServicesWithSorting(int pageIndex, int pageSize, int sortingType)
        {
            if (sortingType == 1)//noSorting
            {
                return DemoContext.RentServices.Where(s => s.Activated == true).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else if (sortingType == 2)// bestGades
            {
                return DemoContext.RentServices.Where(s => s.Activated == true).OrderByDescending(x=>x.Grade).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else if (sortingType == 3)//mostVehicles
            {
                return DemoContext.RentServices.Where(s => s.Activated == true).OrderByDescending(x => x.Vehicles.Count).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else//mostOrders
            {

                return DemoContext.RentServices.Include(v2 => v2.Vehicles).Where(s => s.Activated == true).OrderByDescending(x => x.Vehicles.Sum(o => o.Orders.Count)).Skip((pageIndex - 1) * pageSize).Take(pageSize);


            }
            
        }
        public  RentService GetServiceWithVehicles(int serviceId)
        {
            return DemoContext.RentServices.Include(x => x.Vehicles).Where(r=>r.RentServiceId==serviceId).FirstOrDefault();

        }

        public RentService GetServiceWithComments(int serviceId)
        {
            return DemoContext.RentServices.Include(x => x.Comments).Where(r => r.RentServiceId == serviceId).FirstOrDefault();

        }



    }
}