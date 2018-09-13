using Newtonsoft.Json;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/vehicle")]
    public class VehicleController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IEnumerable<Vehicle> GetVehicles()
        {
            return _unitOfWork.Vehicles.GetAll();
        }

        [HttpGet]
        [Route("vehicles")]
        public IEnumerable<Vehicle> getVehicles()
        {
            return _unitOfWork.Vehicles.GetAll();
        }

        //[HttpGet]
        //[Route("getServiceVehicles/{id}")]
        ////[ResponseType(typeof(Vehicle))]
        //public IEnumerable<Vehicle> getRentServiceVehicles(int id, [FromUri]PagingParameterModel pagingparametermodel)
        //{
        //    //IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.Find(v => v.RentServiceId == id);

        //    //if (vehicles == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //   // return Ok(vehicles);


        //    var source = _unitOfWork.Vehicles.Find(v => v.RentServiceId == id);//unitOfWork.RentServices.GetAll();



        //    // Get's No of Rows Count   
        //    int count = source.Count();

        //    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
        //    int CurrentPage = pagingparametermodel.pageNumber;

        //    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
        //    int PageSize = pagingparametermodel.pageSize;

        //    // Display TotalCount to Records to User  
        //    int TotalCount = count;

        //    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
        //    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

        //    // Returns List of Customer after applying Paging   
        //    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        //    // if CurrentPage is greater than 1 means it has previousPage  
        //    var previousPage = CurrentPage > 1 ? "Yes" : "No";

        //    // if TotalPages is greater than CurrentPage means it has nextPage  
        //    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

        //    // Object which we are going to send in header   
        //    var paginationMetadata = new
        //    {
        //        totalCount = TotalCount,
        //        pageSize = PageSize,
        //        currentPage = CurrentPage,
        //        totalPages = TotalPages,
        //        previousPage,
        //        nextPage
        //    };

        //    // Setting Header  
        //    HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
        //    HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
        //    // Returing List of Customers Collections  
        //    return items;
        //}
        //[HttpPost]
        //[Route("postVehicle")]
        //[ResponseType(typeof(Vehicle))]
        //public IHttpActionResult postVehicle(Vehicle vehicle)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    RentService rentService = _unitOfWork.RentServices.Find(r=>r.RentServiceId==vehicle.RentServiceId).FirstOrDefault();
        //    if (rentService == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Vehicles.Add(vehicle);

        //    try
        //    {
        //        _unitOfWork.Complete();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return BadRequest("Cannot add vehicle.");
        //    }
        //    return Ok(vehicle);
        //}
        [HttpGet]
        [Route("getVehicle/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetVehicler(int id)
        {
            Vehicle vehicle;
            try
            {
                vehicle = _unitOfWork.Vehicles.Find(u => u.VehicleId == id).FirstOrDefault();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot refresh average grade.");
            }
            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpGet]
        [Route("allServiceVehicles/{pageIndex}/{pageSize}/{serviceID}")]
        public IHttpActionResult GetServiceVehicles(int pageIndex, int pageSize, int serviceID)
        {
            //var source = _unitOfWork.Vehicles.Find(x => x.RentServiceId == serviceID);
            var source = _unitOfWork.Vehicles.GetAllWithPics(pageIndex,pageSize, serviceID).ToList();


            // Get's No of Rows Count   
            //int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            // int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = _unitOfWork.Vehicles.CountServiceVehicles(serviceID);

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            // Returns List of Customer after applying Paging   
            //var items = source.ToList();

            

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = pageIndex > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = pageIndex < TotalPages ? "Yes" : "No";

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages,
                //previousPage,
                //nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(source);

        }
        [HttpGet]
        [Route("getServiceVehiclesSort/{pageIndex}/{pageSize}/{serviceID}/{available}/{price}/{type}")]
        public IHttpActionResult GetServiceVehiclesSort(int pageIndex, int pageSize, int serviceID,bool available,string price,int type)
        {
            
            var source = _unitOfWork.Vehicles.GetAllWithPicsUser(pageIndex, pageSize, serviceID,available,price,type).ToList();


           
            int TotalCount = _unitOfWork.Vehicles.CountAllWithPicsUser(serviceID,available, price, type);

           
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
        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("disableVehicle/{vehicleId}/{enabled}")]
        public IHttpActionResult DisableVehicle(int vehicleId, bool enabled)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);

            if (vehicle.Available == true)
            {

                vehicle.Enabled = enabled;
                _unitOfWork.Vehicles.Update(vehicle);
                _unitOfWork.Complete();

                return Ok(vehicle);
            }
            else
            {
                return BadRequest("Vehicle is currently rented");
            }
        }


        [HttpGet]
        [Route("getVehiclePicture")]
        public HttpResponseMessage GetVehiclePicture(string path)
        {
            if (path == null)
            {
                path = "default-placeholder.png";
            }

            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            if (!File.Exists(filePath))
            {
                path = "default-placeholder.png";
                filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            }
            var ext = Path.GetExtension(filePath);

            var contents = File.ReadAllBytes(filePath);

            MemoryStream ms = new MemoryStream(contents);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + ext);

            return response;
        }
        [HttpGet]
        [Route("getVehiclePictures/{vehicleId}")]
        public IHttpActionResult GetVehiclePictures(int vehicleId)
        {

            var photos = _unitOfWork.VehiclePictures.Find(x=>x.VehicleId==vehicleId).ToList();

            if (photos.Count == 0)
            {
                photos.Add(new VehiclePicture() { Data= "default-placeholder.png" ,VehicleId=vehicleId});
            }

            return Ok(photos);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [Route("addVehicle")]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult AddVehicle()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;




            int numberOfImages = Int32.Parse(httpRequest["ImagesNum"]);
            Vehicle vehicle = new Vehicle();
            vehicle.Model = httpRequest["Model"];
            vehicle.Description = httpRequest["Description"];
            vehicle.Manufacturer = httpRequest["Manufacturer"];
            vehicle.YearOfManufacturing = httpRequest["YearOfManufacturing"];
            vehicle.RentServiceId = Int32.Parse(httpRequest["RentServiceId"]);
            vehicle.Available = true;
            vehicle.Enabled = false;
            vehicle.TypeId = Int32.Parse(httpRequest["TypeId"]);
            vehicle.HourlyPrice= double.Parse(httpRequest["HourlyPrice"]);





            _unitOfWork.Vehicles.Add(vehicle);
            _unitOfWork.Complete();

            if (numberOfImages < 1)
            {
               
                _unitOfWork.VehiclePictures.Add(new VehiclePicture() { Data = "default-placeholder.png", VehicleId = vehicle.VehicleId });
                _unitOfWork.Complete();
            }
            else
            {
                for (int i = 0; i < numberOfImages; i++)
                {
                    var postedFile = httpRequest.Files[String.Format("Image{0}", i)];
                    imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                    postedFile.SaveAs(filePath);
                    _unitOfWork.VehiclePictures.Add(new VehiclePicture() { Data = imageName, VehicleId = vehicle.VehicleId });
                    _unitOfWork.Complete();
                }
            }

            return Created("Vehicle was created",vehicle);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("deleteVehicle/{vehicleId}")]
        public IHttpActionResult DeleteOffice(int vehicleId)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

            _unitOfWork.Vehicles.Remove(vehicle);
            _unitOfWork.Complete();

            return Ok();
        }
    }
}
