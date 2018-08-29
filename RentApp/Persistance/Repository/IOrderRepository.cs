using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{
    public interface IOrderRepository:IRepository<Order, int>
    {
        IEnumerable<Order> GetAllUserOrders(int pageIndex, int pageSize, int userId);
        int CountAllUserOrders(int userId);
        Order GetWithVehicles(int orderId);
        IEnumerable<Order> GetServiceOrders(int serviceId);
    }
}
