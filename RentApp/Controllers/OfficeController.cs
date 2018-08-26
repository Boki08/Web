﻿using Newtonsoft.Json;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/office")]
    public class OfficeController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        //readonly IUnitOfWork unitOfWork = new DemoUnitOfWork(new RADBContext());

        public OfficeController(IUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("allServiceOffices/{pageIndex}/{pageSize}/{serviceID}")]
        public IHttpActionResult GetServiceOffices(int pageIndex, int pageSize, int serviceID)
        {
            var source = _unitOfWork.Offices.Find(x => x.RentServiceId == serviceID);



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
        

        [HttpGet]
        [Route("getRentOffice/{officeID}")]
        public IHttpActionResult GetServiceOffice( int officeID)
        {
            var source = _unitOfWork.Offices.Find(x => x.OfficeId == officeID);
 
            return Ok(source);

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
        public HttpResponseMessage AddOffice()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;



            Office office = new Office();
            office.Address = httpRequest["Address"];

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



            _unitOfWork.Offices.Add(office);
            _unitOfWork.Complete();

            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}