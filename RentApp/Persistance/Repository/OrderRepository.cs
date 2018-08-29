using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{
    public class OrderRepository:Repository<Order,int>,IOrderRepository
    {
        public OrderRepository(DbContext context) : base(context)
        {
        }

        protected RADBContext DemoContext { get { return context as RADBContext; } }

        public IEnumerable<Order> GetAllUserOrders(int pageIndex, int pageSize, int userId)
        {
            return DemoContext.Orders.Include(s => s.DepartureOffice).Include(r=>r.ReturnOffice).Include(v=>v.Vehicle).Where(x => x.UserId == userId).ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public int CountAllUserOrders( int userId)
        {
            return DemoContext.Orders.Where(x => x.UserId == userId).ToList().Count;
        }

        public Order GetWithVehicles(int orderId)
        {
            return DemoContext.Orders.Include(v => v.Vehicle).Where(x=>x.OrderId==orderId).FirstOrDefault();
        }

        public IEnumerable<Order> GetServiceOrders(int serviceId)
        {
            return DemoContext.Orders.Include(v => v.Vehicle).Where(x => x.Vehicle.RentServiceId == serviceId);
        }

    }
}
