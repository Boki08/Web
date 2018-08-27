using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class OfficeRepository : Repository<Office, int>, IOfficeRepository
    {
        public OfficeRepository(DbContext context) : base(context)
        {
        }
        protected RADBContext DemoContext { get { return context as RADBContext; } }

        public IEnumerable<Office> GetAll(int pageIndex, int pageSize, int rentServiceId)
        {
            return DemoContext.Offices.Where(x => x.RentServiceId == rentServiceId).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}