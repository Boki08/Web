using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class AppUserRepository : Repository<AppUser, int>, IAppUserRepository
    {
        public AppUserRepository(RADBContext context) : base(context)
        {
        }
        protected RADBContext DemoContext { get { return context as RADBContext; } }
    }
}