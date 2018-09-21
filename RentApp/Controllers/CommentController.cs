using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/comments")]
    public class CommentController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;



        public CommentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [Authorize(Roles = "AppUser")]
        [HttpGet]
        [Route("getComment/{orderId}/{userId}")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetUserComment(int orderId, int userId)
        {
            Comment comment;
            try
            {
                comment = unitOfWork.Comments.Find(x => x.OrderId == orderId).FirstOrDefault();
                if (comment == null)
                {
                    return BadRequest("Comment could not be found.");
                }
            }
            catch
            {
                return BadRequest("Comment could not be found.");
            }
            return Ok(comment);
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet]
        [Route("getCanUserComment/{orderId}/{userId}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetCanUserComment(int orderId, int userId)
        {
            Comment comment;
            try
            {
                comment = unitOfWork.Comments.Find(x => x.OrderId == orderId).FirstOrDefault();
                if (comment == null)
                {
                    Order order = unitOfWork.Orders.Get(orderId);
                    if (order.ReturnDate <= DateTime.Now)
                    {
                        return Ok("canComment");
                    }
                    else
                    {
                        return Ok("can'tCommentYet");
                    }
                }


            }
            catch
            {
                return BadRequest("Comment could not be found.");
            }

            return Ok("commentExists");
        }

        [Authorize(Roles = "AppUser")]
        [HttpPost]
        [Route("postComment")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult PostComment(Comment comment)
        {

            int userId;
                AppUser appUser;
                try
                {
                    var username = User.Identity.Name;

                    var user = unitOfWork.AppUsers.Find(u => u.Email == username).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("Data could not be retrieved, try to relog.");
                }
                appUser = user;

                    userId = appUser.UserId;
            }
            catch
            {
                return BadRequest("User not found, try to relog");
            }

            
            comment.PostedDate = DateTime.Now;

            Order order;
            try
            {
                order = unitOfWork.Orders.GetWithVehicles(comment.OrderId);
                if (order == null)
                {
                    return BadRequest("Order could not be found.");
                }
              
            }
            catch
            {
                return BadRequest("Order could not be found.");
            }

            if (order.ReturnDate > comment.PostedDate)
            {
                return BadRequest("Can't comment before the return date");
            }

            RentService service;


            try
            {
                service = unitOfWork.RentServices.GetServiceWithComments(order.Vehicle.RentServiceId);
                if (service == null)
                {
                    return BadRequest("Rent Service could not be found.");
                }

            }
            catch
            {
                return BadRequest("Rent Service could not be found.");
            }


            int numOfComments = service.Comments.Count;
            double sumGrades = 0;
            if (numOfComments > 0)
            {
                foreach (Comment c in service.Comments)
                {
                    numOfComments++;
                    sumGrades += c.Grade;
                }
            }

            sumGrades += comment.Grade;

            service.Grade = sumGrades / (numOfComments+1);

            comment.Review = comment.Review.Trim();

            service.Comments.Add(comment);


            
            try
            {
                unitOfWork.RentServices.Update(service);
                unitOfWork.Complete();
            }
            catch 
            {
                return BadRequest("Can't refresh average grade.");
            }


            return Created("Comment was posted",comment);
        }

        [HttpGet]
        [Route("getServiceComments/{serviceId}")]
        [ResponseType(typeof(List<Comment>))]
        public IHttpActionResult GetServiceComments(int serviceId)
        {
            List<Comment> comments;
            try
            {
                
                RentService service = unitOfWork.RentServices.GetServiceWithComments(serviceId);
                comments = service.Comments;
                if (comments == null)
                {
                    return BadRequest("There are no comments for this service");
                }
            }
            catch
            {
                return BadRequest("Can't get comments right now");
            }



            return Ok(comments);
        }
    }
}