using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Order
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Vehicle")]
        public int VehicleID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public string DepartureOffice { get; set; }
        public string ReturnOffice { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}