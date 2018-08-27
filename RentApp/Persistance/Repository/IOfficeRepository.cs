using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.Repository
{
    public interface IOfficeRepository : IRepository<Office, int>
    {
        IEnumerable<Office> GetAll(int pageIndex, int pageSize, int rentServiceId);
    }
}
