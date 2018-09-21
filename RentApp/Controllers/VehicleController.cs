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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using RentApp.ETag;
using System.Web.Script.Serialization;

namespace RentApp.Controllers
{
    [RoutePrefix("api/vehicle")]
    public class VehicleController : ApiController
    {
        JsonSerializerSettings setting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


       private readonly IUnitOfWork _unitOfWork;
       

        public VehicleController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

       
        
        [HttpGet]
        [Route("getVehicle/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetVehicle(int id)
        {
            Vehicle vehicle;
            try
            {
                vehicle = _unitOfWork.Vehicles.Find(u => u.VehicleId == id).FirstOrDefault();
            }
            catch 
            {
                return NotFound();
            }
            if (vehicle == null)
            {
                return BadRequest("Vehicle does not exist");
            }

            var jsonObj=JsonConvert.SerializeObject(vehicle, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
            
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));
          

            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) !=null &&HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') == eTag)
                return new StatusCodeResult(HttpStatusCode.NotModified, new HttpRequestMessage());

            return Ok(vehicle);
        }
      
        [Authorize(Roles = "Manager, Admin")]
        [HttpGet]
        [Route("allServiceVehicles/{pageIndex}/{pageSize}/{serviceID}")]
        public IHttpActionResult GetServiceVehicles(int pageIndex, int pageSize, int serviceID)
        {
           
            var source = _unitOfWork.Vehicles.GetAllWithPics(pageIndex,pageSize, serviceID).ToList();

            if (source == null)
            {
                return NotFound();
            }
            else if (source.Count<1)
            {
                return BadRequest("There are no Vehicles");
            }
         

            // Display TotalCount to Records to User  
            int TotalCount = _unitOfWork.Vehicles.CountServiceVehicles(serviceID);

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);


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

            if (source == null)
            {
                return NotFound();
            }
            else if (source.Count < 1)
            {
                return BadRequest("There are no Vehicles");
            }


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

            if (vehicle == null)
            {
                return BadRequest("Office does not exist");
            }
            var jsonObj = JsonConvert.SerializeObject(vehicle, Formatting.None, setting);

            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
          

            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));
                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());

            }


            if (vehicle.Available == true)
            {

                vehicle.Enabled = enabled;
                _unitOfWork.Vehicles.Update(vehicle);
                _unitOfWork.Complete();

                jsonObj = JsonConvert.SerializeObject(vehicle, Formatting.None, setting);


                eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

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
            vehicle.Model = httpRequest["Model"].Trim();
            vehicle.Description = httpRequest["Description"].Trim();
            vehicle.Manufacturer = httpRequest["Manufacturer"].Trim();
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

            return Ok("Vehicle was created");
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [Route("editVehicle")]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult EditVehicle()
        {
            var httpRequest = HttpContext.Current.Request;

            int vehicleId = Int32.Parse(httpRequest["VehicleId"]);
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);

            if (vehicle == null)
            {
                return BadRequest("Office does not exist");
            }

            var jsonObj = JsonConvert.SerializeObject(vehicle, Formatting.None, setting);

            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
           

            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));
                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());
               
            }
            


            int numberOfImages = Int32.Parse(httpRequest["ImagesNum"]);
            vehicle.Model = httpRequest["Model"].Trim();
            vehicle.Description = httpRequest["Description"].Trim();
            vehicle.Manufacturer = httpRequest["Manufacturer"].Trim();
            vehicle.YearOfManufacturing = httpRequest["YearOfManufacturing"];

            vehicle.TypeId = Int32.Parse(httpRequest["TypeId"]);
            vehicle.HourlyPrice = double.Parse(httpRequest["HourlyPrice"]);

            try
            {

                _unitOfWork.Vehicles.Update(vehicle);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Vehicle could not be editer");
            }

            List<VehiclePicture> pictures = _unitOfWork.VehiclePictures.Find(x => x.VehicleId == vehicleId).ToList();


            if (numberOfImages > 0)
            {
                List<PicData> picsData = new List<PicData>();
                for (int i = 0; i < numberOfImages; i++)
                {
                    var postedFile = httpRequest.Files[String.Format("Image{0}", i)];
                    var imgName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).ToArray()).Replace(" ", "-") + Path.GetExtension(postedFile.FileName);
                    if (imgName == "default-placeholder.png")
                        continue;
                    picsData.Add(new PicData() { name = imgName, position = i});
                }
                foreach (VehiclePicture picture in pictures)
                {
                    PicData picData = picsData.Find(x => x.name == picture.Data);
                    if (picData == null)
                    {
                        if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + picture.Data))
                        {
                            File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + picture.Data);
                        }
                        _unitOfWork.VehiclePictures.Remove(picture);
                        _unitOfWork.Complete();
                    }
                    else
                    {
                        picsData.Remove(picData);
                    }
                }

                foreach (PicData picData in picsData)
                {
                    var postedFile = httpRequest.Files[String.Format("Image{0}", picData.position)];
                    picData.name = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    picData.name = picData.name + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + picData.name);
                    postedFile.SaveAs(filePath);
                    _unitOfWork.VehiclePictures.Add(new VehiclePicture() { Data = picData.name, VehicleId = vehicle.VehicleId });
                    _unitOfWork.Complete();
                }

            }
            jsonObj = JsonConvert.SerializeObject(vehicle, Formatting.None, setting);

            eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            return Created("Vehicle was edited", vehicle);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("deleteVehicle/{vehicleId}")]
        public IHttpActionResult DeleteVehicle(int vehicleId)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {
                IEnumerable<VehiclePicture> pictures = _unitOfWork.VehiclePictures.Find(x=>x.VehicleId==vehicleId);
                foreach (VehiclePicture picture in pictures)
                {
                    if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + picture.Data))
                    {
                        File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + picture.Data);
                    }
                    _unitOfWork.VehiclePictures.Remove(picture);
                }

                _unitOfWork.Vehicles.Remove(vehicle);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Vehicle could not be deleted");
            }
            return Ok("Vehicle was deleted");
        }
    }
}
public class PicData
{
    public int position { get; set; }
    public string name { get; set; }
}
