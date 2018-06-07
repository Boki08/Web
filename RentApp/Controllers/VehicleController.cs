using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/vehicle")]
    public class VehicleController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public VehicleController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Vehicle> GetVehicles()
        {
            return unitOfWork.Vehicles.GetAll();
        }

        [HttpGet]
        [Route("vehicles")]
        public IEnumerable<Vehicle> getVehicles()
        {
            return unitOfWork.Vehicles.GetAll();
        }

        [HttpGet]
        [Route("getServiceVehicles/{id}")]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult getRentServiceVehicles(int id)
        {
            IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.Find(v => v.RentServiceId == id);

            if (vehicles == null)
            {
                return NotFound();
            }
            return Ok(vehicles);
        }
        [HttpPost]
        [Route("postVehicle")]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult postVehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RentService rentService = unitOfWork.RentServices.Find(r=>r.RentServiceId==vehicle.RentServiceId).FirstOrDefault();
            if (rentService == null)
            {
                return NotFound();
            }

            unitOfWork.Vehicles.Add(vehicle);

            try
            {
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot add vehicle.");
            }
            return Ok(vehicle);
        }
    }
}
