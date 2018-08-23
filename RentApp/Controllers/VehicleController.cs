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


            var source = _unitOfWork.Vehicles.Find(v => v.RentServiceId == id);//unitOfWork.RentServices.GetAll();



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

            RentService rentService = _unitOfWork.RentServices.Find(r=>r.RentServiceId==vehicle.RentServiceId).FirstOrDefault();
            if (rentService == null)
            {
                return NotFound();
            }

            _unitOfWork.Vehicles.Add(vehicle);

            try
            {
                _unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot add vehicle.");
            }
            return Ok(vehicle);
        }


        [HttpGet]
        [Route("allServiceVehicles/{pageIndex}/{pageSize}/{serviceID}")]
        public IHttpActionResult GetServiceVehicles(int pageIndex, int pageSize, int serviceID)
        {
            var source = _unitOfWork.Vehicles.Find(x => x.RentServiceId == serviceID);



            // Get's No of Rows Count   
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            // int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            

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
                previousPage,
                nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(items);

        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("disableVehicle/{vehicleId}/{enabled}")]
        public IHttpActionResult DisableVehicle(int vehicleId,bool enabled)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);

            
            vehicle.Enabled = enabled;
            _unitOfWork.Vehicles.Update(vehicle);
            _unitOfWork.Complete();

            return Ok(vehicle);
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
            //var content = new MultipartContent();
            //var paths = _unitOfWork.VehiclePictures.Find(x=>x.VehicleId==vehicleId).ToList();



            //if (paths.Count() != 0)
            //{
            //    List<int> IDs = paths.Select(o => o.VehiclePictureId).ToList();
            //    var objectContent = new ObjectContent<List<int>>(IDs, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            //    content.Add(objectContent);
            //    var response = Request.CreateResponse(HttpStatusCode.OK);
            //    foreach (VehiclePicture picture in paths)
            //    {
            //        //var filePath = HttpContext.Current.Server.MapPath("~/Images/" + picture.Data);
            //        //if (!File.Exists(filePath))
            //        //{
            //        //    var path = "default-placeholder.png";
            //        //    filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            //        //}
            //        //var ext = Path.GetExtension(filePath);
            //        //var file1Content = new StreamContent(new FileStream(filePath, FileMode.Open));
            //        //file1Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/" + ext);
            //        //content.Add(file1Content);



            //        //file1Content.Dispose();

            //       // intArray[i] = int.Parse(strArray[i]);
            //        String filePath = HostingEnvironment.MapPath("~/Images/" + picture.Data);
            //        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            //        Image image = Image.FromStream(fileStream);
            //        MemoryStream memoryStream = new MemoryStream();
            //        var ext = Path.GetExtension(filePath);
            //        image.Save(memoryStream, ImageFormat.Jpeg);
            //        response.Content = new ByteArrayContent(memoryStream.ToArray());
            //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            //        fileStream.Close();
            //        memoryStream.Dispose();
            //    }


            //    //response.Content = objectContent;
            //    //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + ext);
            //    return response;
            //}
            //else
            //{

            //       var path = "default-placeholder.png";


            //    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);

            //    var ext = Path.GetExtension(filePath);

            //    var contents = File.ReadAllBytes(filePath);

            //    MemoryStream ms = new MemoryStream(contents);

            //    var response = Request.CreateResponse(HttpStatusCode.OK);
            //    response.Content = new StreamContent(ms);
            //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/" + ext);

            //    return response;
            //}



        }
    }
}
