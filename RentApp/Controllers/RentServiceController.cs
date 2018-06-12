using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RentApp.Controllers
{
    [RoutePrefix("api/rentService")]
    public class RentServiceController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public RentServiceController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<RentService> GetOrders()
        {
            return unitOfWork.RentServices.GetAll();
        }

        [HttpGet]
        [Route("getAll")]
        public IEnumerable<RentService> getOrders()
        {
            return unitOfWork.RentServices.GetAll();
        }
    }
}
