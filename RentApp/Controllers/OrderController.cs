using Newtonsoft.Json;
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
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Order> GetOrders()
        {
            return unitOfWork.Orders.GetAll();
        }

        [HttpGet]
        [Route("orders")]
        public IEnumerable<Order> getOrders()
        {
            return unitOfWork.Orders.GetAll();
        }

        // GET: api/AppUser/5/
        [HttpGet]
        [Route("getMyOrders/{id}")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetMyOrders(int id)
        {
            IEnumerable<Order> myOrders = unitOfWork.Orders.Find(o => o.UserId == id);
            if (myOrders == null)
            {
                return NotFound();
            }

            return Ok(myOrders);
        }

        [HttpPost]
        [Route("postOrder")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult postOrder(Order order)
        {


            if(order.DepartureDate<DateTime.Now || order.ReturnDate<order.DepartureDate || order.ReturnDate < DateTime.Now)
            {
                return BadRequest("You can't add order with these dates!");
            }

            Vehicle vehicle= unitOfWork.Vehicles.Find(v=>v.VehicleId==order.VehicleId).FirstOrDefault();
            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            AppUser appUser;

            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;

                var user = db.Users.Where(u => u.UserName == username).Include(u1 => u1.AppUser).First();
                appUser = user.AppUser;
               
            }
            catch
            {
                return BadRequest("User not found, try to relog");
            }
           // AppUser user = unitOfWork.AppUsers.Find(u => u.UserId == order.UserId).FirstOrDefault();
            if (appUser == null)
            {
                return BadRequest("User not found, try to relog");
            }
            order.UserId = appUser.UserId;

            var hours = (order.ReturnDate - order.DepartureDate).TotalHours;
            order.Price = vehicle.HourlyPrice * hours;

            if (vehicle.Available == true)
            {
                vehicle.Available = false;
                unitOfWork.Vehicles.Update(vehicle);
                unitOfWork.Complete();
            }
            else
            {
                return BadRequest("Vehicle isn't available.");
            }

            unitOfWork.Orders.Add(order);
            try
            {
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot add new order.");
            }
            return Ok(order);
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet]
        [Route("getAllUserOrders/{pageIndex}/{pageSize}")]
        public IHttpActionResult GetAllUserOrders(int pageIndex, int pageSize)
        {
            int userId;
            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;

                var user = db.Users.Where(u => u.UserName == username).Include(u1 => u1.AppUser).First();
                userId = user.AppUser.UserId;

            }
            catch
            {
                return BadRequest();
            }
            //var source = _unitOfWork.Vehicles.Find(x => x.RentServiceId == serviceID);
            var source = unitOfWork.Orders.GetAllUserOrders(pageIndex, pageSize, userId).ToList();

            //foreach (Order order in source)
            //{
            //    order.DepartureOffice = unitOfWork.Offices.Find(x=>x.OfficeId==order.DepartureOfficeId).FirstOrDefault();
            //    order.ReturnOffice = unitOfWork.Offices.Find(x => x.OfficeId == order.ReturnOfficeId).FirstOrDefault();
            //}
            // Get's No of Rows Count   
            //int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            // int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = unitOfWork.Orders.CountAllUserOrders(userId);

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            // Returns List of Customer after applying Paging   
            //var items = source.ToList();



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
                //previousPage,
                //nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(source);

        }

        [Authorize(Roles = "AppUser")]
        [HttpGet]
        [Route("returnVehicle/{orderId}")]
        public IHttpActionResult ReturnVehicle(int orderId)
        {
            int userId;
            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;

                var user = db.Users.Where(u => u.UserName == username).Include(u1 => u1.AppUser).First();
                userId = user.AppUser.UserId;

            }
            catch
            {
                return BadRequest();
            }
            Order order = unitOfWork.Orders.Find(x=>x.OrderId==orderId).FirstOrDefault();

            Vehicle vehicle = order.Vehicle;

            if (vehicle.Available == false && order.UserId==userId)
            {
                vehicle.Available = true;

                unitOfWork.Vehicles.Update(vehicle);

                try
                {
                    unitOfWork.Complete();
                    return Ok(vehicle);
                }
                catch
                {
                    return BadRequest("Can't return the vehicle.");
                }
            }
            return BadRequest("Can't return the vehicle.");
        }
    }
}
