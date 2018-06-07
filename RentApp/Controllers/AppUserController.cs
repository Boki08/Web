using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IHttpActionResult GetAppUser(int id)
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

        [HttpPost]
        [Route("addAppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult addAppUser(AppUser appUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.AppUsers.Add(appUser);

            try
            {
                _unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot refresh average grade.");
            }

            return Ok(appUser);
        }

        [HttpPost]
        [Route("editAppUser/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult editAppUser(AppUser appUser,int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser user = _unitOfWork.AppUsers.Find(u => u.UserId == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            user.BirthDate = appUser.BirthDate;
            user.DocumentPicture = appUser.DocumentPicture;
            user.Email = appUser.Email;
            user.Name = appUser.Name;
            user.Surname = appUser.Surname;
            user.Type = appUser.Type;

            _unitOfWork.AppUsers.Update(user);

            try
            {
                _unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot edit user!");
            }

            return Ok(user);
        }
    }
}
