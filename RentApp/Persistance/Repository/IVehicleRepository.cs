using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{
    public interface IVehicleRepository : IRepository<Vehicle, int>
    {
        IEnumerable<Vehicle> GetAllWithPics(int pageIndex, int pageSize, int rentServiceId);
        int CountServiceVehicles(int rentServiceId);
        IEnumerable<Vehicle> GetAllWithPicsUser(int pageIndex, int pageSize, int rentServiceId, bool available, string price, int type);
    }
}
