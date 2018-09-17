using Newtonsoft.Json;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        public IHttpActionResult GetVehicleTypes()
        {
            var source = _unitOfWork.TypesOfVehicles.GetAll();

            if (source == null || source.Count() < 1)
            {
                return BadRequest("There are no Vehicle Types");
            }
           
            return Ok(source);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getVehicleTypesPaged/{pageIndex}/{pageSize}")]
        public IHttpActionResult GetVehicleTypesPaged(int pageIndex, int pageSize)
        {
            var source= _unitOfWork.TypesOfVehicles.GetAllPaged(pageIndex, pageSize);
            if (source == null || source.Count() < 1)
            {
                return BadRequest("There are no Vehicle Types");
            }
           

            int TotalCount = _unitOfWork.TypesOfVehicles.CountElements();


            int TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages,

            };


            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));


            return Ok(source);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("addVehicleType")]
        public IHttpActionResult AddVehicleType(TypeOfVehicle type)
        {
            IEnumerable<TypeOfVehicle> types = _unitOfWork.TypesOfVehicles.GetAll();

            if (type == null || type.Type==null || type.Type=="")
            {
                return BadRequest("Type can not be empty");
            }

            foreach (TypeOfVehicle t in types)
            {
                if (t.Type == type.Type)
                {
                    return BadRequest("This Vehicle Type already exists");
                }
            }

            _unitOfWork.TypesOfVehicles.Add(type);
            _unitOfWork.Complete();

            return Created("Vehicle type added",type);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("deleteTypeOfVehicle/{typeId}")]
        public IHttpActionResult DeleteTypeOfVehicle(int typeId)
        {
            TypeOfVehicle typeOfVehicle = _unitOfWork.TypesOfVehicles.Get(typeId);
            if (typeOfVehicle == null)
            {
                return NotFound();
            }

            _unitOfWork.TypesOfVehicles.Remove(typeOfVehicle);
            _unitOfWork.Complete();

            return Ok(typeOfVehicle);
        }
    }
}
