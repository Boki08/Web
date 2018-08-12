using Newtonsoft.Json;
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
        public int OrderId { get; set; }
        [Required]
        [ForeignKey("Vehicle")]
        public int VehicleId { get; set; }
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        //[Required]
        //public string DepartureOffice { get; set; }
        //[Required]
        //public string ReturnOffice { get; set; }
        [Required]
        public DateTime DepartureDate { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
        [JsonIgnore]
        public virtual Vehicle Vehicle { get; set; }
        [JsonIgnore]
        public virtual AppUser User { get; set; }
        [JsonIgnore]
        public virtual Office DepartureOffice { get; set; }
        [JsonIgnore]
        public virtual Office ReturnOffice { get; set; }
    }
}