using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Office
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("RentService")]
        public int RentServiceID { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public OfficePicture Picture { get; set; }
    }
}