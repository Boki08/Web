using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Pricing
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("RentService")]
        public int RentServiceID { get; set; }
        public string CreatedBy { get; set; }
        public List<Component> Components { get; set; }
    }
}