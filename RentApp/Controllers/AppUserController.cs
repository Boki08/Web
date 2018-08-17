using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/appUser")]
    public class AppUserController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        //readonly IUnitOfWork unitOfWork = new DemoUnitOfWork(new RADBContext());

        public AppUserController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /* public IEnumerable<AppUser> GetComments()
         {
             return _unitOfWork.AppUsers.GetAll();
         }*/

        [HttpGet]
        [Route("users")]
        public IEnumerable<AppUser> getComments()
        {
            return _unitOfWork.AppUsers.GetAll();
        }

        // GET: api/AppUser/5/
        [HttpGet]
        [Route("getUser/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppIdUser(int id)
        {
            AppUser user;
            try
            {
                user = _unitOfWork.AppUsers.Find(u => u.UserId == id).FirstOrDefault();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot refresh average grade.");
            }
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("getCurrentUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppUser()
        {
            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;
               
                var user = db.Users.Where(u => u.UserName == username).Include(u1 => u1.AppUser).First();
                var appUser = user.AppUser;
                return Ok(appUser);
            }
            catch
            {
                return Ok();
            }
        }

        [HttpPost]
        [Route("addAppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult addAppUser([FromBody]AppUser appUser)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
           
            AppUser hj = _unitOfWork.AppUsers.Find(u => u.UserId == 1).FirstOrDefault();
            _unitOfWork.AppUsers.Add(appUser);

            try
            {
                int a=_unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot refresh average grade.");
            }

            return Ok(appUser);
        }

        [HttpPost]
        [Route("editAppUser")]
        [ResponseType(typeof(AppUser))]
        public HttpResponseMessage EditUser()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;
           
            

            AppUser appUser = _unitOfWork.AppUsers.Get(Int32.Parse(httpRequest["UserId"]));
            appUser.FullName = httpRequest["FullName"];
            appUser.BirthDate = DateTime.Parse(httpRequest["BirthDate"]);
            appUser.Email = httpRequest["Email"];

            if (appUser.DocumentPicture == null || appUser.DocumentPicture == "")
            {
                var postedFile = httpRequest.Files["Image"];
                imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                postedFile.SaveAs(filePath);
                appUser.DocumentPicture = imageName;
            }

            _unitOfWork.AppUsers.Update(appUser);
            _unitOfWork.Complete();

            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}
