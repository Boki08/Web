using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("RentService")]
        public int RentServiceID { get; set; }
        public string Model { get; set; }
        public string YearOfManufacturing { get; set; }
        public string Manufacturer { get; set; }
        public string Pictures { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
    }
}