using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Component
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Vehicle")]
        public int VehicleID { get; set; }
        public float HourlyPrice { get; set; }
    }
}