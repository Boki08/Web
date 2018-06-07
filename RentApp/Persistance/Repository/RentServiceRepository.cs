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
        public RentServiceRepository(RADBContext context) : base(context)
        {
        }

        //public IEnumerable<RentService> GetAll(int pageIndex, int pageSize)
       // {
       //     return DemoContext.RentServices.Skip((pageIndex - 1) * pageSize).Take(pageSize);
       // }
      


        protected RADBContext DemoContext { get { return context as RADBContext; } }
    }
}