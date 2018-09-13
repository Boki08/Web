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
        IEnumerable<RentService> GetAllServicesWithSorting(int pageIndex, int pageSize, int sortingType);
        RentService GetServiceWithVehicles(int serviceId);
        RentService GetServiceWithComments(int serviceId);


    }
}
