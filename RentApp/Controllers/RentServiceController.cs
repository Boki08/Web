﻿using Newtonsoft.Json;
using RentApp.Hubs;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/rentService")]
    public class RentServiceController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentServiceController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IEnumerable<RentService> GetRentServices()
        {
            return _unitOfWork.RentServices.GetAll();
        }

        [HttpGet]
        [Route("getRentService/{serviceId}")]
        public IHttpActionResult GetServiceService(int serviceId)
        {
            var source = _unitOfWork.RentServices.Find(x => x.RentServiceId == serviceId);

            return Ok(source);

        }

        [HttpGet]
        [Route("getAll/{pageIndex}/{pageSize}/{sortType}")]
        public IHttpActionResult getRentServices(int pageIndex, int pageSize, int sortType)
        {
            //var source = _unitOfWork.RentServices.GetAll();
            var items = _unitOfWork.RentServices.GetAllServicesWithSorting(pageIndex, pageSize, sortType).ToList();


            // Get's No of Rows Count   
            //   int count = _unitOfWork.RentServices.CountElements();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            //int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            //int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  


            // Returns List of Customer after applying Paging   
            //var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            //var items = _unitOfWork.RentServices.GetAll(pageIndex, pageSize);

            int count = items.Count();



            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

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
        [HttpPost]
        [Route("addRentService")]
        [ResponseType(typeof(RentService))]
        public IHttpActionResult AddRentService()
        {

            AppUser appUser;

            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;

                var user = db.Users.Where(u => u.UserName == username).Include(a => a.AppUser).First();
                appUser = user.AppUser;

            }
            catch
            {
                return BadRequest("User not found, try to relog");
                // return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (appUser.Activated == false)
            {
                return BadRequest("You can't add new Rent Services right now");
            }

            var httpRequest = HttpContext.Current.Request;

            string imageName = null;



            RentService service = new RentService();
            service.Name = httpRequest["Name"];
            service.Description = httpRequest["Description"];
            service.Email = httpRequest["Email"];
            service.Activated = false;
            service.ServiceEdited = true;


            if (service.Logo == null || service.Logo == "")
            {
                var postedFile = httpRequest.Files["Logo"];
                imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                postedFile.SaveAs(filePath);
                service.Logo = imageName;
            }


            _unitOfWork.RentServices.Add(service);
            _unitOfWork.Complete();

            NotificationsHub.NotifyAdmin("New Rent Service was added");

            return Created("Rent Service was created", service);
        }




        [HttpGet]
        [Route("getAllRentServicesManager/{pageIndex}/{pageSize}/{isApproved}/{noOffices}/{noVehicles}")]
        public IHttpActionResult getAllRentServicesManager(int pageIndex, int pageSize, bool isApproved, bool noOffices, bool noVehicles)
        {
            var source = new List<RentService>();
            if (isApproved)
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == true).ToList();
            }
            else if (noOffices == true && noVehicles == true)
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == false && x.Offices.Count == 0 && x.Vehicles.Count == 0).ToList();
            }
            else if (noOffices == true)
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == false && x.Offices.Count == 0 && x.Vehicles.Count > 0).ToList();
            }
            else if (noVehicles == true)
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == false && x.Offices.Count > 0 && x.Vehicles.Count == 0).ToList();
            }
            else
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == false && x.Offices.Count > 0 && x.Vehicles.Count > 0).ToList();
            }

            // Get's No of Rows Count   
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            //int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //var items = _unitOfWork.RentServices.GetAll(pageIndex, pageSize);



            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(items);
        }


        [HttpGet]
        [Route("getAllRentServicesAdmin/{pageIndex}/{pageSize}/{approved}/{notApproved}/{edited}/{notEdited}/{sort}")]
        public IHttpActionResult getAllRentServicesAdmin(int pageIndex, int pageSize, bool approved, bool notApproved, bool edited, bool notEdited, string sort)
        {
            IEnumerable < RentService> source= new List<RentService>();
            if (approved)
            {
                source = _unitOfWork.RentServices.Find(x => x.Activated == true);
            }
            if (notApproved)
            {
                source = source.Union(_unitOfWork.RentServices.Find(x => x.Activated == false), new RentServiceComparer());
            }
            if (edited)
            {
                source = source.Union(_unitOfWork.RentServices.Find(x => x.ServiceEdited == true), new RentServiceComparer());
                //source = _unitOfWork.RentServices.Find(x => x.ServiceEdited == true ).ToList();
            }
            if (notEdited)
            {
                source = source.Union(_unitOfWork.RentServices.Find(x => x.ServiceEdited == false), new RentServiceComparer());
                // source = _unitOfWork.RentServices.Find(x => x.ServiceEdited == false ).ToList();
            }

            if (!approved && !notApproved && !edited && !notEdited)
            {
                source = _unitOfWork.RentServices.GetAll();

            }

            if(source==null || source.Count() == 0)
            {
                return Ok(source);
            }

            if (sort == "approvedFirst")
            {
                source = source.OrderByDescending(x=>x.Activated==true);
            }
            else if (sort == "notApprovedFirst")
            {
                source = source.OrderByDescending(x => x.Activated == false);
            }
            else if (sort == "editedFirst")
            {
                source = source.OrderByDescending(x => x.ServiceEdited == true);
            }
            else if (sort == "notEditedFirst")
            {
                source = source.OrderByDescending(x => x.ServiceEdited == false);
            }


            // Get's No of Rows Count   
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            //int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //var items = _unitOfWork.RentServices.GetAll(pageIndex, pageSize);



            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(items);
        }




        [HttpGet]
        [Route("getServiceLogo")]
        public HttpResponseMessage GetServiceLogo(string path)
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

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("deleteRentService/{serviceId}")]
        public IHttpActionResult DeleteRentService(int serviceId)
        {
            RentService rentService = _unitOfWork.RentServices.Get(serviceId);
            if (rentService == null)
            {
                return NotFound();
            }

            _unitOfWork.RentServices.Remove(rentService);
            _unitOfWork.Complete();

            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("activateRentService/{serviceId}/{activated}")]
        public IHttpActionResult ActivateRentService(int serviceId,bool activated)
        {
            RentService rentService = _unitOfWork.RentServices.Get(serviceId);
            if (rentService == null)
            {
                return NotFound();
            }

            rentService.Activated = activated;
            rentService.ServiceEdited = false;


            _unitOfWork.RentServices.Update(rentService);
            _unitOfWork.Complete();

            return Ok(string.Format("Rent Service was {0}",activated==true?"activated":"deactivated"));
        }
    }

    public class RentServiceComparer : IEqualityComparer<RentService>
    {

        public bool Equals(RentService x, RentService y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.RentServiceId.Equals(y.RentServiceId);
        }

        public int GetHashCode(RentService obj)
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field. 
            int hashProductCode = obj.RentServiceId.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashProductCode;
        }
    }
}
