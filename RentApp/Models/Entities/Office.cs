using Newtonsoft.Json;
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
        public int OfficeId { get; set; }
        [ForeignKey("RentService")]
        public int RentServiceId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [JsonIgnore]
        public virtual RentService RentService { get; set; }
    }
}