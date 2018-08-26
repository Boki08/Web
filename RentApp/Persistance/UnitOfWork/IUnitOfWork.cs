using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRentServiceRepository RentServices { get; set; }
        ICommentRepository Comments { get; set; }
        IAppUserRepository AppUsers { get; set; }
        IOrderRepository Orders { get; set; }
        IVehicleRepository Vehicles { get; set; }
        IVehiclePictureRepository VehiclePictures { get; set; }
        IOfficeRepository Offices { get; set; }
        ITypeOfVehicleRepository TypesOfVehicles { get; set; }
        int Complete();
    }
}
