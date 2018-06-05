using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class RentService
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }

        public List<Order> Orders { get; set; }
        public List<Office> Offices { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }
}