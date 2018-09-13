using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{

    

    public interface ITypeOfVehicleRepository : IRepository<TypeOfVehicle, int>
    {
        IEnumerable<TypeOfVehicle> GetAllPaged(int pageIndex, int pageSize);
    }
}
