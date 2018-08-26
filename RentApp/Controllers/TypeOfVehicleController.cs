using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RentApp.Controllers
{
    [RoutePrefix("api/typeOfVehicle")]
    public class TypeOfVehicleController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        

        public TypeOfVehicleController(IUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getVehicleTypes")]
        public IEnumerable<TypeOfVehicle> GetVehicleTypes()
        {
            return _unitOfWork.TypesOfVehicles.GetAll();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/AddVehicleType")]
        public IHttpActionResult PutTypeOfVehicle(string type)
        {
            IEnumerable<TypeOfVehicle> types = _unitOfWork.TypesOfVehicles.GetAll();

            if (type == null)
            {
                return Ok();
            }

            foreach (TypeOfVehicle t in types)
            {
                if (t.Type == type)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }

            TypeOfVehicle newType = new TypeOfVehicle() { Type = type };
            _unitOfWork.TypesOfVehicles.Add(newType);
            _unitOfWork.Complete();

            return Ok();
        }
    }
}
