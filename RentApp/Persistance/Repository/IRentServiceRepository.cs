using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{
    public interface IRentServiceRepository: IRepository< RentService, int>
    {
        // IEnumerable<RentService> GetAll(int pageIndex, int pageSize);
        //string GetName();
    }
}
