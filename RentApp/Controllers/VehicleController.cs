using Newtonsoft.Json;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        //[ResponseType(typeof(Vehicle))]
        public IEnumerable<Vehicle> getRentServiceVehicles(int id, [FromUri]PagingParameterModel pagingparametermodel)
        {
            //IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.Find(v => v.RentServiceId == id);

            //if (vehicles == null)
            //{
            //    return NotFound();
            //}
           // return Ok(vehicles);


            var source = unitOfWork.Vehicles.Find(v => v.RentServiceId == id);//unitOfWork.RentServices.GetAll();



            // Get's No of Rows Count   
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = CurrentPage > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return items;
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
