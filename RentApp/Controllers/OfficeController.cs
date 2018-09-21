using Newtonsoft.Json;
using RentApp.ETag;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace RentApp.Controllers
{
    [RoutePrefix("api/office")]
    public class OfficeController : ApiController
    {

        JsonSerializerSettings setting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private readonly IUnitOfWork _unitOfWork;

        public OfficeController(IUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("allServiceOffices/{pageIndex}/{pageSize}/{serviceID}")]
        public IHttpActionResult GetServiceOffices(int pageIndex, int pageSize, int serviceID)
        {
            var source = _unitOfWork.Offices.GetAll(pageIndex,  pageSize,  serviceID);

            if (source == null || source.Count() < 1)
            {
                return BadRequest("There are no Offices");
            }


            // Get's No of Rows Count   
            int count = source.Count();


            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);


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
        [Route("getOffices/{serviceID}")]
        public IHttpActionResult GetAllServiceOffices(int serviceID)
        {
            List<Office> offices;
            try
            {
                offices = _unitOfWork.Offices.Find(x => x.RentServiceId == serviceID).ToList();
            }
            catch
            {
                return NotFound();
            }
            if (offices == null || offices.Count<1)
            {
                return BadRequest("There are no Offices");
            }

            return Ok(offices);
        }

        [HttpGet]
        [Route("getOffice/{officeID}")]
        public IHttpActionResult GetServiceOffice( int officeID)
        {
            

            Office office;
            try
            {
                office = _unitOfWork.Offices.Find(x => x.OfficeId == officeID).FirstOrDefault();
            }
            catch 
            {
                return NotFound();
            }
            if (office == null)
            {
                return BadRequest("Office does not exist");
            }

            var jsonObj = JsonConvert.SerializeObject(office, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));


            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) != null && HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') == eTag)
                return new StatusCodeResult(HttpStatusCode.NotModified, new HttpRequestMessage());

            return Ok(office);
        }


        [HttpGet]
        [Route("getOfficePicture")]
        public HttpResponseMessage GetOfficePicture(string path)
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
        [HttpPost]
        [Route("addOffice")]
        [ResponseType(typeof(Office))]
        public IHttpActionResult AddOffice()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;



            Office office = new Office();
            office.Address = httpRequest["Address"].Trim();

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();
            
            numberFormat.NumberDecimalSeparator = ".";

            office.Latitude = double.Parse(httpRequest["Latitude"], numberFormat);
            office.Longitude = double.Parse(httpRequest["Longitude"], numberFormat);
            office.RentServiceId = Convert.ToInt32(httpRequest["RentServiceId"]);

            var postedFile = httpRequest.Files["Picture"];
            imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
            postedFile.SaveAs(filePath);
            office.Picture = imageName;


            try
            {
                _unitOfWork.Offices.Add(office);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Office could not be added");
            }
            return Created("Office was created", office);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [Route("editOffice")]
        [ResponseType(typeof(Office))]
        public IHttpActionResult EditOffice()
        {
            var httpRequest = HttpContext.Current.Request;

            int officeId = Int32.Parse(httpRequest["OfficeId"]);
            Office office  = _unitOfWork.Offices.Get(officeId);

            if (office == null)
            {
                return BadRequest("Office does not exist");
            }


           var  jsonObj = JsonConvert.SerializeObject(office, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));


            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));
                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());

            }

            string imageName = null;

            office.Address = httpRequest["Address"].Trim();

            var numberFormat = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InstalledUICulture.NumberFormat.Clone();

            numberFormat.NumberDecimalSeparator = ".";

            office.Latitude = double.Parse(httpRequest["Latitude"], numberFormat);
            office.Longitude = double.Parse(httpRequest["Longitude"], numberFormat);
         

            try
            {
                _unitOfWork.Offices.Update(office);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Office could not be editer");
            }


            var postedFile = httpRequest.Files["Picture"];
            if (postedFile != null)
            {
                imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");

                if (office.Picture != imageName && File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + office.Picture))
                {

                    if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + office.Picture))
                    {
                        File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + office.Picture);
                    }
                    

                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                    postedFile.SaveAs(filePath);
                    office.Picture = imageName;
                }
            }

            jsonObj = JsonConvert.SerializeObject(office, Formatting.None, setting);
             eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            return Created("Office was edited", office);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("deleteOffice/{officeId}")]
        public IHttpActionResult DeleteOffice(int officeId)
        {
            Office office = _unitOfWork.Offices.Get(officeId);
            if (office == null)
            {
                return NotFound();
            }
            try
            {
                if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + office.Picture))
                {
                    File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + office.Picture);
                }

                _unitOfWork.Offices.Remove(office);
                _unitOfWork.Complete();
            }
            catch
            {
                return BadRequest("Office could not be deleted");
            }
           

            return Ok("Office was deleted");
        }
    }
}
