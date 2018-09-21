using Newtonsoft.Json;
using RentApp.ETag;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace RentApp.Controllers
{
    [RoutePrefix("api/typeOfVehicle")]
    public class TypeOfVehicleController : ApiController
    {
        JsonSerializerSettings setting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
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

            type.Type = type.Type.Trim();

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
                return BadRequest("This Vehicle Type cant be found");
            }

           

            try
            {
                _unitOfWork.TypesOfVehicles.Remove(typeOfVehicle);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Vehicle Type can't be added");
            }

            return Ok(typeOfVehicle);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getTypeOfVehicle/{typeId}")]
        public IHttpActionResult GetTypeOfVehicle(int typeId)
        {
            TypeOfVehicle typeOfVehicle = _unitOfWork.TypesOfVehicles.Get(typeId);
            if (typeOfVehicle == null)
            {
                return BadRequest("This Vehicle Type cant be found");
            }

            var jsonObj = JsonConvert.SerializeObject(typeOfVehicle, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));

            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) != null && HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') == eTag)
                return new StatusCodeResult(HttpStatusCode.NotModified, new HttpRequestMessage());

            return Ok(typeOfVehicle);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("editTypeOfVehicle")]
        public IHttpActionResult EditTypeOfVehicle(TypeOfVehicle type)
        {
            TypeOfVehicle typeOfVehicle = _unitOfWork.TypesOfVehicles.Get(type.TypeId);
            if (typeOfVehicle == null)
            {
                return BadRequest("This Vehicle Type can't be found");
            }

            var jsonObj = JsonConvert.SerializeObject(typeOfVehicle, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));



            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());

            }

            typeOfVehicle.Type = type.Type.Trim();

            try
            {
                _unitOfWork.TypesOfVehicles.Update(typeOfVehicle);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Vehicle Type can't be edited");
            }

            jsonObj = JsonConvert.SerializeObject(typeOfVehicle, Formatting.None, setting);
            eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));

            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            return Ok(typeOfVehicle);
        }
    }
}
