using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Unity.Attributes;

namespace RentApp.Persistance.UnitOfWork
{
    public class DemoUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
      
        [Dependency]
        public IRentServiceRepository RentServices { get; set; }

        [Dependency]
        public ICommentRepository Comments { get; set; }
        [Dependency]
        public IAppUserRepository AppUsers { get; set; }
        [Dependency]
        public IOrderRepository Orders { get; set; }
        
        public DemoUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}