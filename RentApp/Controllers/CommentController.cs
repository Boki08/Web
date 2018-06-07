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

        public IEnumerable<Comment> GetComments()
        {
            return unitOfWork.Comments.GetAll();
        }


        [HttpGet]
        [Route("")]
        public IEnumerable<Comment> getComments()
        {
            return unitOfWork.Comments.GetAll();
        }

        // GET: api/Comments/5/3
        [HttpGet]
        [Route("comments/{id1}/{id2}")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int id1, int id2)
        {
            Comment comment = unitOfWork.Comments.Find(cm => cm.OrderId == id1 && cm.UserId == id2).FirstOrDefault();
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        [Route("Comments")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult postComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RADBContext db = new RADBContext();
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var userRole = user.Roles.First().RoleId;
            var role = db.Roles.FirstOrDefault(r => r.Id == userRole);
            bool isAppUser = role.Name.Equals("AppUser");


            // var user = unitOfWork.AppUsers.Find(u=>u.Name == User.Identity.Name).FirstOrDefault();
            // var userRole = user.Type;
            // bool isAppUser = userRole.Equals("AppUser");


            // using (var context = new RADBContext())
            // {
            //  var orders = from b in context.Orders
            //                    where (b.UserId == comment.UserId &&
            //                           b.OrderId == comment.OrderId)
            //                   select b;

            IEnumerable<Order> orders = unitOfWork.Orders.Find(o => o.UserId == comment.UserId && o.OrderId == comment.OrderId);
            foreach (Order o in orders)
            {
                if (o.ReturnDate < DateTime.Now)
                {
                    try
                    {
                        db.Comments.Add(comment);
                        db.SaveChanges();
                    }

                    catch
                    {
                        return BadRequest("You can add comments after the return!");
                    }

                    IEnumerable<Comment> comments = unitOfWork.Comments.Find(cm => cm.Order.Vehicle.RentServiceId == comment.Order.Vehicle.RentServiceId);

                    double sum = 0;
                    double averageGrade;

                    foreach (Comment c in comments)
                    {
                        sum += c.Grade;
                    }
                    averageGrade = sum / comments.Count();

                    RentService rentService = unitOfWork.RentServices.Find(r=>r.RentServiceId==comment.Order.Vehicle.RentServiceId).FirstOrDefault();

                    if (rentService == null)
                    {
                        return BadRequest("Cannot refresh average grade.");
                    }
                    rentService.Grade = averageGrade;

                    unitOfWork.RentServices.Update(rentService);
                    try
                    {
                        unitOfWork.Complete();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest("Cannot refresh average grade.");
                    }

                    //!!!!!!return!!!!!!!!!!!!

                }
                // }

               
            }
            return Unauthorized();
        }
    }
}