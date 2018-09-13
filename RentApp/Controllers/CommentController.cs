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

        public IEnumerable<Comment> GetComments()//?????
        {
            return unitOfWork.Comments.GetAll();
        }


        [HttpGet]
        [Route("comments")]
        public IEnumerable<Comment> getComments()
        {
            return unitOfWork.Comments.GetAll();
        }

        // GET: api/Comments/5/3
        //[HttpGet]
        //[Route("getComments/{orderId}")]
        //[ResponseType(typeof(Comment))]
        //public IHttpActionResult GetComment(int id1)
        //{
        //    Comment comment = unitOfWork.Comments.Find(cm => cm.OrderId == id1).FirstOrDefault();
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(comment);
        //}


        [HttpGet]
        [Route("getComment/{orderId}/{userId}")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetUserComment(int orderId, int userId)
        {
            Comment comment = unitOfWork.Comments.Find(x => x.OrderId == orderId && x.UserId == userId).FirstOrDefault();
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpGet]
        [Route("getCanUserComment/{orderId}/{userId}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetCanUserComment(int orderId, int userId)
        {
            Comment comment = unitOfWork.Comments.Find(x => x.OrderId == orderId && x.UserId == userId).FirstOrDefault();

            if (comment == null)
            {
                Order order = unitOfWork.Orders.Find(x => x.OrderId == orderId).FirstOrDefault();
                if (order.ReturnDate <= DateTime.Now)
                {
                    return Ok("canComment");
                }
                else
                {
                    return Ok("can'tCommentYet");
                }
            }

            return Ok("commentExists");
        }

        [HttpPost]
        [Route("postComment")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult PostComment(Comment comment)
        {
            //RentService service = unitOfWork.RentServices.GetServiceWithVehicles(1);


            ////List<Order> orders = unitOfWork.Orders.GetServiceOrders(service.RentServiceId).ToList();
            //List<Order> orders = new List<Order>();
            //foreach (Vehicle vehicle in service.Vehicles)
            //{
            //    orders.AddRange(vehicle.Orders);
            //}


            RADBContext db = new RADBContext();
            int userId;
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                userId = user.AppUserId;
            }
            catch
            {
                return BadRequest("User not found, try to relog");
            }


            //var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            //int userId =  user.AppUserId;
            comment.UserId = userId;
            comment.PostedDate = DateTime.Now;

            Order order = unitOfWork.Orders.GetWithVehicles(comment.OrderId);

            if(order.ReturnDate> comment.PostedDate)
            {
                return BadRequest("Can't comment before the return date");
            }

            
            //RentService service = unitOfWork.RentServices.Find(x=>x.RentServiceId == order.Vehicle.RentServiceId).ToList().FirstOrDefault();
            // RentService service1 = unitOfWork.RentServices.GetServiceWithVehicles(order.Vehicle.RentServiceId);
            RentService service = unitOfWork.RentServices.GetServiceWithComments(order.Vehicle.RentServiceId);


            //List<Order> orders = unitOfWork.Orders.GetServiceOrders(service.RentServiceId).ToList();
            //List<Order> orders=new List<Order>();
            //foreach (Vehicle vehicle in service.Vehicles)
            //{
            //    orders.AddRange(vehicle.Orders);
            //}
            //orders=service.Vehicles

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

            service.Comments.Add(comment);//?????????????????



            //unitOfWork.Comments.Add(comment);??????????????????????????????????
            //try
            //{
            //    unitOfWork.Complete();
            //}
            //catch
            //{
            //    return BadRequest("Can't add comment.");
            //}

            unitOfWork.RentServices.Update(service);
            try
            {
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