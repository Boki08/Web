using RentApp.Models.Entities;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RentService service = unitOfWork.RentServices.Find(r=>r.RentServiceId==order.Vehicle.RentServiceId).FirstOrDefault();

            if (service == null)
            {
                return NotFound();
            }

            if(order.DepartureDate<DateTime.Now || order.ReturnDate<order.DepartureDate || order.ReturnDate < DateTime.Now)
            {
                return BadRequest("You can add order with these dates!");
            }

            Vehicle vehicle= unitOfWork.Vehicles.Find(v=>v.VehicleId==order.VehicleId).FirstOrDefault();
            if (vehicle == null)
            {
                return NotFound();
            }
            AppUser user = unitOfWork.AppUsers.Find(u => u.UserId == order.UserId).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
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
    }
}
